namespace LandingWallet.Components.Wallet;

/// <summary>
/// Reporte individual de Looker Studio para las cards modulares del dashboard.
/// Las URLs son públicas (no son secretos) y vienen tal como las generó
/// Looker Studio —incluido /page/…—; se asignan al src sin transformación.
/// Width/Height son el lienzo NATIVO oficial de Looker Studio para cada
/// embed: TODOS los reportes se rediseñaron a360×340 (relación nativa), y
/// Wallet renderiza el viewport con aspect-ratio: 360 / 340. No es un ancho
/// físico máximo: en desktop el iframe se expande proporcionalmente.
/// NOTA: si un reporte tiene páginas internas más altas que su lienzo,
/// Looker gestiona ese exceso con su propio scroll interno; ampliar la
/// altura del iframe más allá de la relación nativa rompe el render de
/// Looker (solo pinta la cabecera), por eso se conserva la dimensión
/// oficial.
/// </summary>
public sealed record WalletLookerReport(
    string Title,
    string ReportUrl,
    int Width,
    int Height,
    Shared.IconName Icon);

/// <summary>
/// Catálogo único de los reportes individuales de Looker (fuente única de
/// verdad de títulos/URLs, al estilo de WalletUserNavigation).
///
/// ESTADO DE EMBEDS (verificado): 9/9 habilitados por el propietario.
/// El reporte "Estado de las transacciones" (4c540c2e…/page/iVx9F) estuvo
/// bloqueado temporalmente — el donut nativo cubrió su métrica — y ya fue
/// habilitado e integrado: la analítica Admin es 100 % Looker.
/// </summary>
public static class WalletLookerReports
{
    /// <summary>Reportes del rol USUARIO, en orden de jerarquía visual.</summary>
    public static readonly IReadOnlyList<WalletLookerReport> User =
    [
        new("Ingresos y egresos por período",
            "https://datastudio.google.com/embed/reporting/bca31527-5a60-407b-b730-743828b73122/page/Qkx9F",
            360, 340, Shared.IconName.Transfer),
        new("Evolución de la actividad",
            "https://datastudio.google.com/embed/reporting/e2f26d53-8108-4eca-97af-c1ae1709091a/page/Bhx9F",
            360, 340, Shared.IconName.TrendingUp),
        new("Distribución de egresos",
            "https://datastudio.google.com/embed/reporting/6e31e135-6c63-4f45-97e3-c361fbf04f2c/page/oix9F",
            360, 340, Shared.IconName.Analytics),
    ];

    /// <summary>
    /// Reportes del rol ADMIN (6 de 6). Orden de composición: fila1
    /// sistema, fila2 operación, fila3 auditoría.
    /// </summary>
    public static readonly IReadOnlyList<WalletLookerReport> Admin =
    [
        new("Volumen transaccionado",
            "https://datastudio.google.com/embed/reporting/12ac99fc-2a85-4f64-b400-28b62515923c/page/rNx9F",
            360, 340, Shared.IconName.Currency),
        new("Altas de usuarios",
            "https://datastudio.google.com/embed/reporting/62bd0c11-3c70-4511-8ed4-6d1156a2e7d4/page/gUx9F",
            360, 340, Shared.IconName.Users),
        new("Transacciones por tipo",
            "https://datastudio.google.com/embed/reporting/7498c7ff-092a-4e8f-a1f1-c18b39b628fd/page/uWx9F",
            360, 340, Shared.IconName.Purchase),
        new("Errores de auditoría por tipo",
            "https://datastudio.google.com/embed/reporting/3a0def99-fb45-459a-8edf-cd2e8d12388d/page/4Xx9F",
            360, 340, Shared.IconName.Audit),
        new("Evolución de errores",
            "https://datastudio.google.com/embed/reporting/0f4f5f7f-a453-4a7f-9124-2c96618e37d0/page/6Zx9F",
            360, 340, Shared.IconName.Warning),
        new("Estado de las transacciones",
            "https://datastudio.google.com/embed/reporting/4c540c2e-64d6-4c40-a6f2-4f367afa6b9c/page/iVx9F",
            360, 340, Shared.IconName.Transfer),
    ];

    /// <summary>
    /// URL pública del reporte (sin /embed) para el enlace "Abrir reporte":
    /// /embed es el único tramo que distingue a la variante embebible.
    /// </summary>
    public static string FullReportUrl(WalletLookerReport report)
        => report.ReportUrl.Replace("/embed/", "/", StringComparison.OrdinalIgnoreCase);
}
