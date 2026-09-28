namespace LandingWallet.Components.Wallet.Data;

/// <summary>
/// DTOs crudos de BigQuery: reflejan 1:1 las columnas de cada vista.
///
/// NO son el modelo de presentación. La capa de acceso a datos los llena y el
/// servicio los mapea a los modelos de presentación existentes. Se mantienen
/// separados para que un cambio de columna en BigQuery se resuelva acá y no se
/// propague a los componentes.
///
/// Todos los importes llegan ya resueltos por la vista: ninguna capa de la
/// aplicación los recalcula, los convierte ni los suma entre monedas.
/// </summary>

/// <summary>
/// Fila de <c>wallet_mart.vw_dashboard_usuario</c>: el resumen del período del
/// usuario y su saldo por moneda. Cada moneda viene en su propia columna; nunca
/// se consolidan ARS y USD.
///
/// La vista expone el resumen del período para AMBAS monedas
/// (<c>*_periodo_ars</c> y <c>*_periodo_usd</c>, verificado contra
/// INFORMATION_SCHEMA) y los indicadores <c>tiene_cuenta_ars</c> /
/// <c>tiene_cuenta_usd</c> que distinguen "cuenta inexistente" de "cuenta con
/// saldo 0".
/// </summary>
public sealed record WalletDashboardUserRow(
    int IdUsuario,
    string NombreUsuario,
    DateOnly? InicioPeriodo,
    DateOnly? FinPeriodo,
    int CantidadCuentas,
    int CantidadCuentasActivas,
    bool TieneCuentaArs,
    bool TieneCuentaUsd,
    decimal SaldoActualArs,
    decimal SaldoActualUsd,
    decimal IngresosPeriodoArs,
    decimal EgresosPeriodoArs,
    decimal BalancePeriodoArs,
    decimal IngresosPeriodoUsd,
    decimal EgresosPeriodoUsd,
    decimal BalancePeriodoUsd,
    int CantidadMovimientosPeriodo);

/// <summary>
/// Fila de <c>wallet_mart.vw_saldo_cuentas</c>: una cuenta del usuario con su
/// saldo actual, ya calculado por la fuente.
/// </summary>
public sealed record WalletAccountRow(
    long IdCuenta,
    string Alias,
    string Cbu,
    string EstadoCuenta,
    string NombreTipoCuenta,
    string CodigoMoneda,
    string NombreMoneda,
    decimal SaldoActual,
    decimal Creditos,
    decimal Debitos);

/// <summary>
/// Fila de <c>wallet_mart.vw_transacciones_reporting</c>: un movimiento.
///
/// La dirección se deriva comparando el usuario consultado con el origen y el
/// destino REALES de la transacción; no se asume por el signo del importe.
/// </summary>
public sealed record WalletMovementRow(
    long IdTransaccion,
    DateOnly? FechaLocal,
    decimal Monto,
    string Estado,
    string? Descripcion,
    string NombreTipoTransaccion,
    string CodigoMoneda,
    long? IdUsuarioOrigen,
    long? IdUsuarioDestino);

/// <summary>
/// Fila de <c>wallet_mart.vw_inversiones_reporting</c>: una inversión de una
/// cuenta del usuario. La columna de tasa llega como fracción tal como la
/// informa la vista.
/// </summary>
public sealed record WalletInvestmentRow(
    long IdInversion,
    long IdCuenta,
    string NombreTipoInversion,
    decimal MontoInvertido,
    decimal TasaRendimiento,
    DateOnly? FechaInicio,
    DateOnly? FechaVencimiento,
    int? PlazoDias,
    decimal InteresGanado,
    decimal MontoResultante,
    string Estado);

/// <summary>
/// Totales del panel de administración, cada uno obtenido de su propia fuente.
/// Los volúmenes van SIEMPRE separados por moneda.
/// </summary>
public sealed record WalletAdminTotals(
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

/// <summary>
/// Fila de agrupación por <c>estado</c> (transacciones). Se usa para los
/// totales por estado sin depender del orden de las filas.
/// </summary>
public sealed record WalletStatusCountRow(string Estado, int Total);

/// <summary>
/// Fila de volumen aprobado por moneda. La moneda es parte de la clave porque
/// ARS y USD nunca se suman.
/// </summary>
public sealed record WalletVolumeRow(string CodigoMoneda, decimal Volumen);
