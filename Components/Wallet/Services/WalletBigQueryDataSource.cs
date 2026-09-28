using System.Globalization;
using Google.Cloud.BigQuery.V2;
using LandingWallet.Components.Wallet.Data;
using Microsoft.Extensions.Options;

namespace LandingWallet.Components.Wallet.Services;

/// <summary>
/// Interfaz de la capa de acceso a datos de BigQuery.
///
/// Existe para que <see cref="WalletDashboardService"/> dependa de una
/// abstracciÃ³n y sea verificable sin credenciales reales. NO es un repositorio
/// por vista: es UNA capa comÃºn, como se aprobÃ³.
/// </summary>
public interface IWalletBigQueryDataSource
{
    /// <summary>Resumen del perÃ­odo y saldos del usuario.</summary>
    Task<WalletDashboardUserRow?> GetDashboardUserAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Cuentas del usuario. Si <paramref name="currencyCode"/> es ARS o USD,
    /// solo las de esa moneda; si es <c>null</c>, todas (sin mezclar saldos:
    /// cada cuenta conserva su propia moneda). La moneda se valida contra la
    /// allowlist antes de llegar a la query.
    /// </summary>
    Task<IReadOnlyList<WalletAccountRow>> GetAccountsAsync(string? currencyCode = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Movimientos del usuario. Si <paramref name="currencyCode"/> es ARS o USD,
    /// solo los de esa moneda; si es <c>null</c>, todas las monedas. Del mÃ¡s
    /// reciente al mÃ¡s antiguo. El filtro por moneda es parte de la consulta.
    /// </summary>
    Task<IReadOnlyList<WalletMovementRow>> GetMovementsAsync(string? currencyCode = null, int count = 20, CancellationToken cancellationToken = default);

    /// <summary>Inversiones de las cuentas del usuario.</summary>
    Task<IReadOnlyList<WalletInvestmentRow>> GetInvestmentsAsync(CancellationToken cancellationToken = default);

    /// <summary>Totales del panel de administraciÃ³n.</summary>
    Task<WalletAdminTotals> GetAdminTotalsAsync(CancellationToken cancellationToken = default);

    /// <summary>Transacciones agrupadas por estado.</summary>
    Task<IReadOnlyList<WalletStatusCountRow>> GetTransactionsByStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>Transacciones agrupadas por tipo.</summary>
    Task<IReadOnlyList<WalletStatusCountRow>> GetTransactionsByTypeAsync(CancellationToken cancellationToken = default);

    /// <summary>Errores de auditorÃ­a agrupados por cÃ³digo.</summary>
    Task<IReadOnlyList<WalletStatusCountRow>> GetAuditErrorsByCodeAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Capa ÃšNICA de acceso a BigQuery.
///
/// RESPONSABILIDAD: ejecutar consultas de SOLO LECTURA contra las vistas
/// acordadas y devolver DTOs crudos. NO conoce reglas de negocio, NO calcula
/// saldos, NO convierte monedas, NO suma ARS + USD y NO conoce componentes.
///
/// CONSULTAS PARAMETRIZADAS: el <c>id_usuario</c> se pasa siempre como
/// <see cref="BigQueryParameter"/>, nunca interpolado en el texto de la query.
///
/// CREDENCIALES: usa <see cref="BigQueryClient"/> con Application Default
/// Credentials. Si no hay credenciales disponibles, el cliente falla al
/// ejecutar y la excepciÃ³n sube al servicio, que la convierte en estado Error.
/// NUNCA se devuelven datos de relleno ante un fallo.
/// </summary>
public sealed class WalletBigQueryDataSource : IWalletBigQueryDataSource
{
    private readonly WalletBigQueryOptions _options;

    // El cliente se crea de forma PEREZOSA (Lazy). Motivo medido: crear el
    // cliente resuelve las Application Default Credentials y LANZA si no estÃ¡n
    // configuradas. Si eso ocurriera al construir el servicio (inyecciÃ³n de
    // dependencias), la excepciÃ³n escaparÃ­a del try/catch del componente y la
    // pantalla responderÃ­a 500 en lugar de mostrar su estado Error. Con Lazy, la
    // resoluciÃ³n ocurre dentro de la primera consulta, que sÃ­ estÃ¡ dentro del
    // try/catch: el fallo se convierte en ERROR visible, como exige el DiseÃ±o.
    private readonly Lazy<BigQueryClient> _client;

    public WalletBigQueryDataSource(IOptions<WalletBigQueryOptions> options)
    {
        _options = options.Value;
        _client = new Lazy<BigQueryClient>(() => BigQueryClient.Create(_options.ProjectId),
                                           LazyThreadSafetyMode.ExecutionAndPublication);
    }

    private string Mart(string view) => $"`{_options.ProjectId}.{_options.MartDataset}.{view}`";

    private string Core(string table) => $"`{_options.ProjectId}.{_options.CoreDataset}.{table}`";

    /// <summary>ParÃ¡metro del usuario demo, siempre parametrizado.</summary>
    private BigQueryParameter DemoUser() => new("id_usuario", BigQueryDbType.Int64, _options.DemoUserId);

    /// <inheritdoc />
    public async Task<WalletDashboardUserRow?> GetDashboardUserAsync(
        CancellationToken cancellationToken = default)
    {
        // El resumen del período existe para AMBAS monedas en la vista
        // (verificado contra INFORMATION_SCHEMA): se leen las columnas *_ars y
        // *_usd reales. Nunca se convierte una moneda en la otra.
        var sql = $"""
            SELECT
                id_usuario,
                nombre_usuario,
                inicio_periodo,
                fin_periodo,
                cantidad_cuentas,
                cantidad_cuentas_activas,
                tiene_cuenta_ars,
                tiene_cuenta_usd,
                saldo_actual_ars,
                saldo_actual_usd,
                ingresos_periodo_ars,
                egresos_periodo_ars,
                balance_periodo_ars,
                ingresos_periodo_usd,
                egresos_periodo_usd,
                balance_periodo_usd,
                cantidad_movimientos_periodo
            FROM {Mart("vw_dashboard_usuario")}
            WHERE id_usuario = @id_usuario
            LIMIT 1
            """;

        var rows = await _client.Value.ExecuteQueryAsync(
            sql, [DemoUser()], cancellationToken: cancellationToken);

        foreach (var row in rows)
        {
            return new WalletDashboardUserRow(
                IdUsuario: GetInt(row, "id_usuario"),
                NombreUsuario: GetString(row, "nombre_usuario"),
                InicioPeriodo: GetDate(row, "inicio_periodo"),
                FinPeriodo: GetDate(row, "fin_periodo"),
                CantidadCuentas: GetInt(row, "cantidad_cuentas"),
                CantidadCuentasActivas: GetInt(row, "cantidad_cuentas_activas"),
                TieneCuentaArs: GetBool(row, "tiene_cuenta_ars"),
                TieneCuentaUsd: GetBool(row, "tiene_cuenta_usd"),
                SaldoActualArs: GetDecimal(row, "saldo_actual_ars"),
                SaldoActualUsd: GetDecimal(row, "saldo_actual_usd"),
                IngresosPeriodoArs: GetDecimal(row, "ingresos_periodo_ars"),
                EgresosPeriodoArs: GetDecimal(row, "egresos_periodo_ars"),
                BalancePeriodoArs: GetDecimal(row, "balance_periodo_ars"),
                IngresosPeriodoUsd: GetDecimal(row, "ingresos_periodo_usd"),
                EgresosPeriodoUsd: GetDecimal(row, "egresos_periodo_usd"),
                BalancePeriodoUsd: GetDecimal(row, "balance_periodo_usd"),
                CantidadMovimientosPeriodo: GetInt(row, "cantidad_movimientos_periodo"));
        }

        return null;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WalletAccountRow>> GetAccountsAsync(
        string? currencyCode = null,
        CancellationToken cancellationToken = default)
    {
        // La moneda se valida contra la allowlist ANTES de construir la query:
        // un código arbitrario nunca llega a BigQuery. null = todas las cuentas.
        if (currencyCode is not null)
        {
            WalletCurrency.RequireAllowed(currencyCode);
        }

        var sql = $"""
            SELECT
                id_cuenta,
                alias,
                cbu,
                estado_cuenta,
                nombre_tipo_cuenta,
                codigo_moneda,
                nombre_moneda,
                saldo_actual,
                creditos,
                debitos
            FROM {Mart("vw_saldo_cuentas")}
            WHERE id_usuario = @id_usuario
            {(currencyCode is null ? "" : "AND codigo_moneda = @codigo_moneda")}
            ORDER BY codigo_moneda
            """;

        var parameters = new List<BigQueryParameter> { DemoUser() };
        if (currencyCode is not null)
        {
            parameters.Add(new BigQueryParameter("codigo_moneda", BigQueryDbType.String, currencyCode));
        }

        var rows = await _client.Value.ExecuteQueryAsync(
            sql, parameters, cancellationToken: cancellationToken);

        var result = new List<WalletAccountRow>();
        foreach (var row in rows)
        {
            result.Add(new WalletAccountRow(
                IdCuenta: GetLong(row, "id_cuenta"),
                Alias: GetString(row, "alias"),
                Cbu: GetString(row, "cbu"),
                EstadoCuenta: GetString(row, "estado_cuenta"),
                NombreTipoCuenta: GetString(row, "nombre_tipo_cuenta"),
                CodigoMoneda: GetString(row, "codigo_moneda"),
                NombreMoneda: GetString(row, "nombre_moneda"),
                SaldoActual: GetDecimal(row, "saldo_actual"),
                Creditos: GetDecimal(row, "creditos"),
                Debitos: GetDecimal(row, "debitos")));
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WalletMovementRow>> GetMovementsAsync(
        string? currencyCode = null,
        int count = 20,
        CancellationToken cancellationToken = default)
    {
        // La moneda se valida contra la allowlist ANTES de construir la query.
        // null = todas las monedas (comportamiento previo).
        if (currencyCode is not null)
        {
            WalletCurrency.RequireAllowed(currencyCode);
        }

        // Un movimiento pertenece al usuario si es origen o destino REAL de la
        // transacciÃ³n (no se infiere por el signo del importe). El filtro por
        // moneda es parte de la consulta: la UI nunca filtra por texto.
        var sql = $"""
            SELECT
                id_transaccion,
                fecha_local,
                monto,
                estado,
                descripcion,
                nombre_tipo_transaccion,
                codigo_moneda,
                id_usuario_origen,
                id_usuario_destino
            FROM {Mart("vw_transacciones_reporting")}
            WHERE (id_usuario_origen = @id_usuario OR id_usuario_destino = @id_usuario)
            {(currencyCode is null ? "" : "AND codigo_moneda = @codigo_moneda")}
            ORDER BY fecha_transaccion_utc DESC
            LIMIT @limite
            """;

        var parameters = new List<BigQueryParameter>
        {
            DemoUser(),
            new BigQueryParameter("limite", BigQueryDbType.Int64, Math.Max(0, count)),
        };
        if (currencyCode is not null)
        {
            parameters.Add(new BigQueryParameter("codigo_moneda", BigQueryDbType.String, currencyCode));
        }

        var rows = await _client.Value.ExecuteQueryAsync(sql, parameters, cancellationToken: cancellationToken);

        var result = new List<WalletMovementRow>();
        foreach (var row in rows)
        {
            result.Add(new WalletMovementRow(
                IdTransaccion: GetLong(row, "id_transaccion"),
                FechaLocal: GetDate(row, "fecha_local"),
                Monto: GetDecimal(row, "monto"),
                Estado: GetString(row, "estado"),
                Descripcion: GetNullableString(row, "descripcion"),
                NombreTipoTransaccion: GetString(row, "nombre_tipo_transaccion"),
                CodigoMoneda: GetString(row, "codigo_moneda"),
                IdUsuarioOrigen: GetNullableLong(row, "id_usuario_origen"),
                IdUsuarioDestino: GetNullableLong(row, "id_usuario_destino")));
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WalletInvestmentRow>> GetInvestmentsAsync(
        CancellationToken cancellationToken = default)
    {
        // Las inversiones se resuelven por PERTENENCIA de la cuenta, no por IDs
        // hardcodeados: la subconsulta descubre las cuentas del usuario.
        var sql = $"""
            SELECT
                id_inversion,
                id_cuenta,
                nombre_tipo_inversion,
                monto_invertido,
                tasa_rendimiento,
                fecha_inicio,
                fecha_vencimiento,
                plazo_dias,
                interes_ganado,
                monto_resultante,
                estado
            FROM {Mart("vw_inversiones_reporting")}
            WHERE id_cuenta IN (
                SELECT id_cuenta
                FROM {Mart("vw_saldo_cuentas")}
                WHERE id_usuario = @id_usuario
            )
            ORDER BY fecha_inicio DESC
            """;

        var rows = await _client.Value.ExecuteQueryAsync(
            sql, [DemoUser()], cancellationToken: cancellationToken);

        var result = new List<WalletInvestmentRow>();
        foreach (var row in rows)
        {
            result.Add(new WalletInvestmentRow(
                IdInversion: GetLong(row, "id_inversion"),
                IdCuenta: GetLong(row, "id_cuenta"),
                NombreTipoInversion: GetString(row, "nombre_tipo_inversion"),
                MontoInvertido: GetDecimal(row, "monto_invertido"),
                TasaRendimiento: GetDecimal(row, "tasa_rendimiento"),
                FechaInicio: GetDate(row, "fecha_inicio"),
                FechaVencimiento: GetDate(row, "fecha_vencimiento"),
                PlazoDias: GetNullableInt(row, "plazo_dias"),
                InteresGanado: GetDecimal(row, "interes_ganado"),
                MontoResultante: GetDecimal(row, "monto_resultante"),
                Estado: GetString(row, "estado")));
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<WalletAdminTotals> GetAdminTotalsAsync(
        CancellationToken cancellationToken = default)
    {
        // Una sola consulta con subconsultas escalares: menos viajes y un Ãºnico
        // punto de lectura consistente. Los volÃºmenes van separados por moneda.
        var sql = $"""
            SELECT
                (SELECT COUNT(*) FROM {Core("dim_usuarios")}) AS total_usuarios,
                (SELECT COUNT(*) FROM {Core("dim_usuarios")} WHERE LOWER(TRIM(estado)) = 'activo') AS usuarios_activos,
                (SELECT COUNT(*) FROM {Core("dim_cuentas")}) AS total_cuentas,
                (SELECT COUNT(*) FROM {Core("vw_cuentas_activas")}) AS cuentas_activas,
                (SELECT COUNT(*) FROM {Core("fct_transacciones")}) AS total_transacciones,
                (SELECT COUNT(*) FROM {Core("fct_transacciones")} WHERE UPPER(TRIM(estado)) = 'APROBADA') AS aprobadas,
                (SELECT COUNT(*) FROM {Core("fct_transacciones")} WHERE UPPER(TRIM(estado)) = 'PENDIENTE') AS pendientes,
                (SELECT COUNT(*) FROM {Core("fct_transacciones")} WHERE UPPER(TRIM(estado)) = 'RECHAZADA') AS rechazadas,
                (SELECT COUNT(*) FROM {Core("fct_inversiones")}) AS total_inversiones,
                (SELECT COUNT(*) FROM {Core("fct_inversiones")} WHERE UPPER(TRIM(estado)) = 'ACTIVA') AS inversiones_activas,
                (SELECT COUNT(*) FROM {Core("fct_auditorias")}) AS total_auditorias,
                (SELECT COUNT(*) FROM {Core("fct_auditorias")} WHERE codigo_error IS NOT NULL) AS auditorias_con_error,
                (SELECT COUNT(*) FROM {Core("fct_alertas")}) AS total_alertas,
                (SELECT COUNT(*) FROM {Core("fct_alertas")} WHERE fecha_lectura IS NULL) AS alertas_no_leidas
            """;

        var rows = await _client.Value.ExecuteQueryAsync(sql, null, cancellationToken: cancellationToken);

        foreach (var row in rows)
        {
            return new WalletAdminTotals(
                TotalUsers: GetInt(row, "total_usuarios"),
                ActiveUsers: GetInt(row, "usuarios_activos"),
                TotalAccounts: GetInt(row, "total_cuentas"),
                ActiveAccounts: GetInt(row, "cuentas_activas"),
                TotalTransactions: GetInt(row, "total_transacciones"),
                ApprovedTransactions: GetInt(row, "aprobadas"),
                PendingTransactions: GetInt(row, "pendientes"),
                RejectedTransactions: GetInt(row, "rechazadas"),
                ApprovedVolumeArs: await GetApprovedVolumeAsync("ARS", cancellationToken),
                ApprovedVolumeUsd: await GetApprovedVolumeAsync("USD", cancellationToken),
                TotalInvestments: GetInt(row, "total_inversiones"),
                ActiveInvestments: GetInt(row, "inversiones_activas"),
                TotalAudits: GetInt(row, "total_auditorias"),
                AuditsWithError: GetInt(row, "auditorias_con_error"),
                TotalAlerts: GetInt(row, "total_alertas"),
                UnreadAlerts: GetInt(row, "alertas_no_leidas"));
        }

        // Sin fila: no se inventan ceros como si fueran datos reales.
        throw new InvalidOperationException(
            "La consulta de totales del panel de administraciÃ³n no devolviÃ³ resultados.");
    }

    /// <summary>
    /// Volumen aprobado de UNA moneda. Se consulta por separado para que ARS y
    /// USD nunca se sumen entre sÃ­.
    /// </summary>
    private async Task<decimal> GetApprovedVolumeAsync(string currencyCode, CancellationToken cancellationToken)
    {
        var sql = $"""
            SELECT COALESCE(SUM(monto), 0) AS volumen
            FROM {Mart("vw_transacciones_reporting")}
            WHERE UPPER(TRIM(estado)) = 'APROBADA' AND codigo_moneda = @moneda
            """;

        var parameters = new[] { new BigQueryParameter("moneda", BigQueryDbType.String, currencyCode) };
        var rows = await _client.Value.ExecuteQueryAsync(sql, parameters, cancellationToken: cancellationToken);

        foreach (var row in rows)
        {
            return GetDecimal(row, "volumen");
        }

        return 0m;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WalletStatusCountRow>> GetTransactionsByStatusAsync(
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT estado AS etiqueta, COUNT(*) AS total
            FROM {Mart("vw_transacciones_reporting")}
            GROUP BY estado
            ORDER BY total DESC
            """;

        return await GetGroupedAsync(sql, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WalletStatusCountRow>> GetTransactionsByTypeAsync(
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT nombre_tipo_transaccion AS etiqueta, COUNT(*) AS total
            FROM {Mart("vw_transacciones_reporting")}
            GROUP BY nombre_tipo_transaccion
            ORDER BY total DESC
            """;

        return await GetGroupedAsync(sql, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WalletStatusCountRow>> GetAuditErrorsByCodeAsync(
        CancellationToken cancellationToken = default)
    {
        var sql = $"""
            SELECT codigo_error AS etiqueta, COUNT(*) AS total
            FROM {Core("fct_auditorias")}
            WHERE codigo_error IS NOT NULL
            GROUP BY codigo_error
            ORDER BY total DESC
            """;

        return await GetGroupedAsync(sql, cancellationToken);
    }

    /// <summary>Ejecuta una agrupaciÃ³n etiqueta/total y la materializa.</summary>
    private async Task<IReadOnlyList<WalletStatusCountRow>> GetGroupedAsync(
        string sql,
        CancellationToken cancellationToken)
    {
        var rows = await _client.Value.ExecuteQueryAsync(sql, null, cancellationToken: cancellationToken);

        var result = new List<WalletStatusCountRow>();
        foreach (var row in rows)
        {
            result.Add(new WalletStatusCountRow(
                Estado: GetString(row, "etiqueta"),
                Total: GetInt(row, "total")));
        }

        return result;
    }

    // ------------------------------------------------------------------------
    // Lectura tolerante de columnas.
    //
    // Se verifica la existencia de la columna contra el Schema real de la fila
    // antes de leerla con el indexador. AsÃ­, si una vista no expone una columna,
    // se obtiene null y el valor cae al default del tipo en lugar de lanzar una
    // KeyNotFoundException opaca en mitad del mapeo.
    //
    // BigQuery puede devolver INT64 como long o como string segÃºn la vista, de
    // ahÃ­ las conversiones. Nunca se devuelve un valor por defecto silencioso
    // para un fallo de BigQuery: un fallo de la consulta sube como excepciÃ³n.
    // ------------------------------------------------------------------------

    private static bool HasColumn(BigQueryRow row, string column)
        => row.Schema?.Fields?.Any(field =>
               string.Equals(field.Name, column, StringComparison.OrdinalIgnoreCase)) == true;

    private static object? Value(BigQueryRow row, string column)
        => HasColumn(row, column) ? row[column] : null;

    private static string GetString(BigQueryRow row, string column)
        => Value(row, column)?.ToString() ?? string.Empty;

    private static string? GetNullableString(BigQueryRow row, string column)
    {
        var value = Value(row, column)?.ToString();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static long GetLong(BigQueryRow row, string column)
    {
        var value = Value(row, column);
        return value switch
        {
            null => 0L,
            long l => l,
            int i => i,
            decimal d => (long)d,
            BigQueryNumeric n => (long)n.ToDecimal(LossOfPrecisionHandling.Truncate),
            string s => long.Parse(s, CultureInfo.InvariantCulture),
            _ => throw new InvalidCastException(
                $"La columna '{column}' no se puede convertir a entero (tipo {value.GetType().Name})."),
        };
    }

    private static long? GetNullableLong(BigQueryRow row, string column)
        => Value(row, column) is null ? null : GetLong(row, column);

    private static int GetInt(BigQueryRow row, string column) => (int)GetLong(row, column);

    private static int? GetNullableInt(BigQueryRow row, string column)
        => Value(row, column) is null ? null : GetInt(row, column);

    private static bool GetBool(BigQueryRow row, string column)
    {
        var value = Value(row, column);
        return value switch
        {
            null => false,
            bool b => b,
            long l => l != 0,
            int i => i != 0,
            string s => bool.TryParse(s, out var parsed) && parsed,
            _ => false,
        };
    }

    private static decimal GetDecimal(BigQueryRow row, string column)
    {
        var value = Value(row, column);
        return value switch
        {
            null => 0m,
            decimal d => d,
            BigQueryNumeric n => n.ToDecimal(LossOfPrecisionHandling.Truncate),
            long l => l,
            int i => i,
            double db => (decimal)db,
            string s => decimal.Parse(s, CultureInfo.InvariantCulture),
            _ => throw new InvalidCastException(
                $"La columna '{column}' no se puede convertir a decimal (tipo {value.GetType().Name})."),
        };
    }

    private static DateOnly? GetDate(BigQueryRow row, string column)
    {
        var value = Value(row, column);
        return value switch
        {
            null => null,
            DateOnly d => d,
            DateTime dt => DateOnly.FromDateTime(dt),
            string s when DateOnly.TryParse(s, CultureInfo.InvariantCulture, out var parsed) => parsed,
            _ => null,
        };
    }
}
