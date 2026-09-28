using LandingWallet.Components.Wallet.Data;
using Microsoft.Extensions.Options;

namespace LandingWallet.Components.Wallet.Services;

/// <summary>
/// Implementación de <see cref="IWalletDashboardService"/> sobre BigQuery REAL.
///
/// TRAZABILIDAD:
///   BigQuery (wallet_mart / wallet_core)
///     → <see cref="IWalletBigQueryDataSource"/>   (SQL + lectura de filas)
///     → WalletDashboardService                    (mapeo a modelos) ← este archivo
///     → IWalletDashboardService
///     → componentes Blazor
///
/// RESPONSABILIDAD ACOTADA: consultar el datasource y MAPEAR los DTO crudos a
/// los modelos de presentación. NO contiene SQL, NO abre clientes de BigQuery,
/// NO inventa valores y NO tiene fallback a datos demo.
///
/// ESTADOS: si BigQuery falla, la excepción se propaga tal cual. El servicio no
/// la convierte en lista vacía ni en ceros: la pantalla muestra Error. Una
/// lista vacía legítima (por ejemplo, el usuario no tiene inversiones) llega
/// como lista vacía y la pantalla muestra Empty.
/// </summary>
public sealed class WalletDashboardService : IWalletDashboardService
{
    private readonly IWalletBigQueryDataSource _dataSource;
    private readonly WalletBigQueryOptions _options;

    public WalletDashboardService(
        IWalletBigQueryDataSource dataSource,
        IOptions<WalletBigQueryOptions> options)
    {
        _dataSource = dataSource;
        _options = options.Value;
    }

    /// <inheritdoc />
    public string DisplayUserName => _options.DemoDisplayName;

    /// <inheritdoc />
    public async Task<IReadOnlyList<WalletPresentationAccount>> GetAccountsAsync(
        string? currencyCode = null,
        CancellationToken cancellationToken = default)
    {
        var rows = await _dataSource.GetAccountsAsync(currencyCode, cancellationToken);

        return [.. rows.Select(row => new WalletPresentationAccount(
            Type: row.NombreTipoCuenta,
            Alias: row.Alias,
            Cbu: row.Cbu,
            CurrencyCode: row.CodigoMoneda,
            CurrencyName: row.NombreMoneda,
            Status: row.EstadoCuenta,
            Balance: row.SaldoActual,
            Credits: row.Creditos,
            Debits: row.Debitos))];
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WalletPresentationMovement>> GetRecentMovementsAsync(
        string? currencyCode = null,
        int count = 20,
        CancellationToken cancellationToken = default)
    {
        var rows = await _dataSource.GetMovementsAsync(currencyCode, count, cancellationToken);

        return [.. rows.Select(MapMovement)];
    }

    /// <summary>
    /// Mapea un movimiento crudo al modelo de presentación.
    ///
    /// DIRECCIÓN: se resuelve comparando el usuario demo con el ORIGEN y el
    /// DESTINO reales de la transacción. Si el usuario es el origen, el dinero
    /// sale (egreso); si es el destino, entra (ingreso). No se infiere por el
    /// signo del importe, que la vista informa siempre en positivo.
    /// </summary>
    private WalletPresentationMovement MapMovement(WalletMovementRow row)
    {
        var isOutgoing = row.IdUsuarioOrigen == _options.DemoUserId
                         && row.IdUsuarioDestino != _options.DemoUserId;

        // El texto principal prioriza el tipo REAL de la transacción. La
        // descripción del dataset puede llevar un rótulo interno ("... mock"),
        // así que se omite cuando es así en lugar de mostrarla como dato.
        var description = string.IsNullOrWhiteSpace(row.Descripcion)
            || row.Descripcion.Contains("mock", StringComparison.OrdinalIgnoreCase)
                ? null
                : row.Descripcion;

        return new WalletPresentationMovement(
            Date: row.FechaLocal ?? DateOnly.FromDateTime(DateTime.UtcNow),
            Type: row.NombreTipoTransaccion,
            Direction: isOutgoing ? WalletMovementDirection.Outgoing : WalletMovementDirection.Incoming,
            CurrencyCode: row.CodigoMoneda,
            Amount: row.Monto,
            Description: description);
    }

    /// <inheritdoc />
    public async Task<WalletPeriodSummary?> GetPeriodSummaryAsync(
        string currencyCode,
        CancellationToken cancellationToken = default)
    {
        // La moneda se valida contra la allowlist ANTES de llegar a la fuente.
        WalletCurrency.RequireAllowed(currencyCode);

        var row = await _dataSource.GetDashboardUserAsync(cancellationToken);

        if (row is null)
        {
            // Sin fila para el usuario: la pantalla lo trata como "sin resumen",
            // no como un resumen en cero.
            return null;
        }

        // El resumen es de UNA moneda. Los importes llegan resueltos por la
        // vista en columnas separadas por moneda (*_ars / *_usd): no se
        // recalculan ni se convierten.
        var (income, expense, balance) = currencyCode switch
        {
            WalletCurrency.Ars => (row.IngresosPeriodoArs, row.EgresosPeriodoArs, row.BalancePeriodoArs),
            WalletCurrency.Usd => (row.IngresosPeriodoUsd, row.EgresosPeriodoUsd, row.BalancePeriodoUsd),
            _ => throw new ArgumentOutOfRangeException(
                nameof(currencyCode), currencyCode,
                "Moneda no permitida. Solo se admiten ARS y USD."),
        };

        return new WalletPeriodSummary(
            PeriodLabel: null,
            CurrencyCode: currencyCode,
            Income: income,
            Expense: expense,
            Balance: balance);
    }

    /// <inheritdoc />
    public async Task<WalletPeriodRange?> GetPeriodRangeAsync(
        CancellationToken cancellationToken = default)
    {
        var row = await _dataSource.GetDashboardUserAsync(cancellationToken);

        // El rango sale de la fuente. Si no está informado, no se asume un mes.
        return row?.InicioPeriodo is { } start && row.FinPeriodo is { } end
            ? new WalletPeriodRange(start, end)
            : null;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WalletPresentationInvestment>> GetInvestmentsAsync(
        CancellationToken cancellationToken = default)
    {
        var rows = await _dataSource.GetInvestmentsAsync(cancellationToken);

        // Se agrupa por TIPO porque la tarjeta presenta una fila por tipo. Si el
        // usuario no tiene inversiones, el agrupamiento queda vacío y la pantalla
        // muestra su estado vacío: no se inventa ninguna colocación.
        return [.. rows
            .GroupBy(row => row.NombreTipoInversion)
            .Select(group => new WalletPresentationInvestment(
                Type: group.Key,
                Quantity: group.Count(),
                Active: group.Count(row => row.Estado.Equals("ACTIVA", StringComparison.OrdinalIgnoreCase)),
                TotalInvested: group.Sum(row => row.MontoInvertido),
                InterestEarned: group.Sum(row => row.InteresGanado),
                ResultingTotal: group.Sum(row => row.MontoResultante),
                AverageRate: group.Average(row => row.TasaRendimiento)))];
    }

    /// <inheritdoc />
    public async Task<WalletAdminKpis> GetAdminKpisAsync(CancellationToken cancellationToken = default)
    {
        var totals = await _dataSource.GetAdminTotalsAsync(cancellationToken);

        return new WalletAdminKpis(
            TotalUsers: totals.TotalUsers,
            ActiveUsers: totals.ActiveUsers,
            TotalAccounts: totals.TotalAccounts,
            ActiveAccounts: totals.ActiveAccounts,
            TotalTransactions: totals.TotalTransactions,
            ApprovedTransactions: totals.ApprovedTransactions,
            PendingTransactions: totals.PendingTransactions,
            RejectedTransactions: totals.RejectedTransactions,
            ApprovedVolumeArs: totals.ApprovedVolumeArs,
            ApprovedVolumeUsd: totals.ApprovedVolumeUsd,
            TotalInvestments: totals.TotalInvestments,
            ActiveInvestments: totals.ActiveInvestments,
            TotalAudits: totals.TotalAudits,
            AuditsWithError: totals.AuditsWithError,
            TotalAlerts: totals.TotalAlerts,
            UnreadAlerts: totals.UnreadAlerts);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WalletChartSegment>> GetTransactionsByStatusAsync(
        CancellationToken cancellationToken = default)
        => MapSegments(await _dataSource.GetTransactionsByStatusAsync(cancellationToken));

    /// <inheritdoc />
    public async Task<IReadOnlyList<WalletChartSegment>> GetTransactionsByTypeAsync(
        CancellationToken cancellationToken = default)
        => MapSegments(await _dataSource.GetTransactionsByTypeAsync(cancellationToken));

    /// <inheritdoc />
    public async Task<IReadOnlyList<WalletChartSegment>> GetAuditErrorsByCodeAsync(
        CancellationToken cancellationToken = default)
        => MapSegments(await _dataSource.GetAuditErrorsByCodeAsync(cancellationToken));

    /// <summary>
    /// Convierte agrupaciones etiqueta/total al segmento de visualización. No
    /// agrega ni omite categorías: la fuente decide qué existe.
    /// </summary>
    private static IReadOnlyList<WalletChartSegment> MapSegments(IReadOnlyList<WalletStatusCountRow> rows)
        => [.. rows.Select(row => new WalletChartSegment(row.Estado, row.Total))];
}
