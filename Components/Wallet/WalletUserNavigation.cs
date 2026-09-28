using LandingWallet.Components.Shared;

namespace LandingWallet.Components.Wallet;

/// <summary>
/// Menú canónico de la App Wallet (usuario y admin comparten la misma
/// navegación final).
///
/// Es la única definición de la navegación: las pantallas no repiten la lista
/// ni indican a mano cuál es la sección actual. La sección activa la resuelve
/// la navegación por ruta.
///
/// PRESENTACIÓN (la lista es única; cambia solo la presentación CSS):
/// · Desktop (≥1024px): sidebar completa con el grupo DASHBOARDS.
/// · Mobile: barra inferior con SOLO 4 accesos (Inicio, Cuentas, Transferir,
///   Más). Los ítems DesktopOnly —Dashboard Usuario, Dashboard Admin e
///   Inversiones— se ocultan y se alcanzan desde Más.
///
/// Inicio apunta a la Landing ("/"): la Landing es la presentación general,
/// y los dashboards son las experiencias de cada rol.
/// </summary>
public static class WalletUserNavigation
{
    /// <summary>Ruta de la Landing (presentación general).</summary>
    public const string LandingRoute = "/";

    /// <summary>Ruta del dashboard del usuario.</summary>
    public const string DashboardRoute = "/wallet/dashboard";

    /// <summary>Ruta del panel de administración.</summary>
    public const string AdminRoute = "/admin/dashboard";

    /// <summary>Ruta de la sección de inversiones del usuario.</summary>
    public const string InvestmentsRoute = "/wallet/inversiones";

    /// <summary>Secciones de la navegación, en su orden de aparición.</summary>
    public static readonly IReadOnlyList<WalletNavigationItem> Items =
    [
        new("Inicio", IconName.Home, Href: LandingRoute),

        // Grupo secundario (solo desktop): label no clickeable + accesos.
        new("DASHBOARDS", IsSectionLabel: true, DesktopOnly: true),
        new("Dashboard Usuario", IconName.Wallet, Href: DashboardRoute,
            DesktopOnly: true, IsSectionChild: true),
        new("Dashboard Admin", IconName.Dashboard, Href: AdminRoute,
            DesktopOnly: true, IsSectionChild: true),

        new("Cuentas", IconName.Accounts, Href: "/wallet/cuentas"),
        new("Transferir", IconName.Transfer, Href: "/wallet/transferir"),
        new("Inversiones", IconName.Analytics, Href: InvestmentsRoute, DesktopOnly: true),
        new("Más", IconName.More, Href: "/wallet/mas"),
    ];
}
