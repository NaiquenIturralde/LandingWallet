using LandingWallet.Components.Wallet.Data;

namespace LandingWallet.Components.Wallet.Services;

/// <summary>
/// Acceso a los datos financieros que consume la App Wallet (rol usuario).
///
/// AÍSLA LA FUENTE: las pantallas dependen solo de esta interfaz, nunca de
/// BigQuery ni de ninguna capa de acceso. La implementación vigente
/// (<see cref="WalletDashboardService"/>) resuelve todo contra BigQuery real.
///
/// Los métodos son asíncronos porque la fuente es remota: las pantallas ya
/// contemplan carga, error y vacío.
///
/// La interfaz NO calcula saldos, NO convierte monedas y NO suma monedas
/// distintas: presenta lo que la fuente informa.
///
/// EXTENSIÓN DEL CONTRATO (justificada en la etapa de integración de BigQuery):
/// se agregaron <see cref="GetPeriodRangeAsync"/>, <see cref="GetAdminKpisAsync"/>
/// y los agrupamientos para visualizaciones, porque la fuente real expone el
/// rango del período (antes era una constante hardcodeada) y los KPIs del panel
/// de administración (antes vivían como valores literales en el código). Sin
/// ellos, esas pantallas seguirían dependiendo de datos ajenos a BigQuery.
/// </summary>
public interface IWalletDashboardService
{
    /// <summary>
    /// Nombre visible del usuario de la demo. Sale de la configuración, no de
    /// la fuente: la vista almacena otro nombre y solo se sobrescribe el rótulo.
    /// </summary>
    string DisplayUserName { get; }

    /// <summary>
    /// Cuentas del usuario. Si <paramref name="currencyCode"/> es ARS o USD,
    /// solo las de esa moneda; si es <c>null</c>, todas (cada cuenta conserva
    /// su propia moneda, nunca se suman). El saldo llega resuelto por la fuente
    /// (<c>saldo_actual</c>): no se deriva en la interfaz.
    /// </summary>
    Task<IReadOnlyList<WalletPresentationAccount>> GetAccountsAsync(
        string? currencyCode = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Movimientos más recientes del usuario. Si <paramref name="currencyCode"/>
    /// es ARS o USD, solo los de esa moneda; si es <c>null</c>, todas las
    /// monedas. Del más nuevo al más antiguo. El orden, el recorte y el filtro
    /// por moneda los resuelve la fuente: la lista de movimientos solo presenta
    /// lo que recibe.
    /// </summary>
    /// <param name="currencyCode">Moneda de los movimientos (ARS, USD o null).</param>
    /// <param name="count">Cantidad máxima de movimientos a devolver.</param>
    Task<IReadOnlyList<WalletPresentationMovement>> GetRecentMovementsAsync(
        string? currencyCode = null,
        int count = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resumen del período de UNA moneda (ARS o USD): ingresos, egresos y su
    /// balance, ya resueltos por la fuente. Devuelve <c>null</c> cuando no hay
    /// un período que informar, y la pantalla lo trata como "sin resumen", no
    /// como cero.
    ///
    /// Es de una sola moneda: la interfaz no consolida ARS y USD.
    /// </summary>
    Task<WalletPeriodSummary?> GetPeriodSummaryAsync(
        string currencyCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Rango temporal REAL que cubre el resumen (<c>inicio_periodo</c> /
    /// <c>fin_periodo</c>). Devuelve <c>null</c> si la fuente no lo informa:
    /// la pantalla usa entonces un título genérico en lugar de asumir un mes.
    /// </summary>
    Task<WalletPeriodRange?> GetPeriodRangeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Inversiones del usuario. Puede ser una lista vacía: no tener inversiones
    /// es un estado normal, no un error.
    /// </summary>
    Task<IReadOnlyList<WalletPresentationInvestment>> GetInvestmentsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// KPIs del panel de administración, cada uno desde su fuente real. No hay
    /// valores literales: si BigQuery falla, el error se propaga y la pantalla
    /// lo informa; nunca se devuelven ceros como si fueran datos.
    /// </summary>
    Task<WalletAdminKpis> GetAdminKpisAsync(CancellationToken cancellationToken = default);

    /// <summary>Transacciones agrupadas por estado (para visualizaciones).</summary>
    Task<IReadOnlyList<WalletChartSegment>> GetTransactionsByStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>Transacciones agrupadas por tipo (para visualizaciones).</summary>
    Task<IReadOnlyList<WalletChartSegment>> GetTransactionsByTypeAsync(CancellationToken cancellationToken = default);

    /// <summary>Errores de auditoría agrupados por código (para visualizaciones).</summary>
    Task<IReadOnlyList<WalletChartSegment>> GetAuditErrorsByCodeAsync(CancellationToken cancellationToken = default);
}
