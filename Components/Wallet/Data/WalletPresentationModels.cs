namespace LandingWallet.Components.Wallet.Data;

/// <summary>
/// Dirección de un movimiento según el modelo funcional.
/// Es una clasificación de presentación: no se deriva ni se recalcula en la UI.
/// </summary>
public enum WalletMovementDirection
{
    /// <summary>Crédito: suma al saldo de la cuenta.</summary>
    Incoming,

    /// <summary>Débito: resta al saldo de la cuenta.</summary>
    Outgoing,
}

/// <summary>
/// Cuenta de Wallet ya resuelta para presentación.
/// El saldo llega calculado por la fuente de datos (<c>saldo_actual</c>);
/// la UI nunca lo recalcula ni lo suma entre monedas.
/// </summary>
public sealed record WalletPresentationAccount(
    string Type,
    string Alias,
    string Cbu,
    string CurrencyCode,
    string CurrencyName,
    string Status,
    decimal Balance,
    decimal Credits,
    decimal Debits);

/// <summary>Movimiento de una cuenta, ya resuelto para presentación.</summary>
public sealed record WalletPresentationMovement(
    DateOnly Date,
    string Type,
    WalletMovementDirection Direction,
    string CurrencyCode,
    decimal Amount,
    string? Description);

/// <summary>Segmento de una visualización de datos (categoría + valor real).</summary>
public sealed record WalletChartSegment(string Label, decimal Value);

/// <summary>
/// Resumen del período de una cuenta: ingresos, egresos y su balance.
///
/// Los tres importes llegan resueltos por la fuente de datos, igual que
/// <c>saldo_actual</c>: la interfaz no los estima, no los recalcula y no los
/// convierte. El balance es la diferencia entre ingresos y egresos del período
/// y <b>no</b> es el saldo de la cuenta: son dos conceptos distintos.
///
/// Un resumen corresponde a una sola moneda. Nunca se consolidan ARS y USD en
/// un mismo balance.
/// </summary>
/// <param name="PeriodLabel">
/// Nombre del período según la fuente (por ejemplo, "septiembre"). Si es
/// <c>null</c>, la pantalla usa un título genérico: no se inventa un mes.
/// </param>
/// <param name="CurrencyCode">Moneda del resumen. No se mezcla con otra.</param>
/// <param name="Income">Ingresos (créditos) del período.</param>
/// <param name="Expense">Egresos (débitos) del período.</param>
/// <param name="Balance">Balance del período: ingresos menos egresos.</param>
public sealed record WalletPeriodSummary(
    string? PeriodLabel,
    string CurrencyCode,
    decimal Income,
    decimal Expense,
    decimal Balance);

/// <summary>
/// Rango temporal que cubre el resumen del período, ya resuelto por la fuente
/// (<c>inicio_periodo</c> / <c>fin_periodo</c> de vw_dashboard_usuario).
///
/// Existe para poder mostrar el período REAL en lugar de asumir un mes: antes
/// el rótulo salía de una constante. Si la fuente no informa el rango, la
/// pantalla usa un título genérico.
/// </summary>
public sealed record WalletPeriodRange(DateOnly Start, DateOnly End);

/// <summary>Saldo del usuario por moneda, resuelto por la fuente.</summary>
public sealed record WalletCurrencyBalance(string CurrencyCode, decimal Balance);

/// <summary>Inversión agregada, ya resuelta para presentación.</summary>
public sealed record WalletPresentationInvestment(
    string Type,
    int Quantity,
    int Active,
    decimal TotalInvested,
    decimal InterestEarned,
    decimal ResultingTotal,
    decimal AverageRate);

/// <summary>
/// KPIs consolidados del panel de administración, ya resueltos por la fuente.
///
/// Cada métrica proviene de su propia consulta a BigQuery: no hay valores
/// literales en el código. Los volúmenes van SIEMPRE separados por moneda
/// porque ARS y USD no se suman entre sí.
/// </summary>
public sealed record WalletAdminKpis(
    int TotalUsers,
    int ActiveUsers,
    int TotalAccounts,
    int ActiveAccounts,
    int TotalTransactions,
    int ApprovedTransactions,
    int PendingTransactions,
    int RejectedTransactions,
    decimal ApprovedVolumeArs,
    decimal ApprovedVolumeUsd,
    int TotalInvestments,
    int ActiveInvestments,
    int TotalAudits,
    int AuditsWithError,
    int TotalAlerts,
    int UnreadAlerts);
