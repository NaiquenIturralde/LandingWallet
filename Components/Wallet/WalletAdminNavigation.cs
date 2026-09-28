using LandingWallet.Components.Shared;

namespace LandingWallet.Components.Wallet;

/// <summary>
/// Menú histórico del rol admin (App Wallet).
///
/// La navegación final se UNIFICOU en <see cref="WalletUserNavigation"/>: la
/// ruta /admin/dashboard y la presentación embebida de la Landing usan esa
/// lista única (sidebar desktop con el grupo DASHBOARDS; bottom nav mobile de
///4 accesos). Esta lista se conserva documentada por trazabilidad pero ya no
/// la consume ninguna pantalla.
///
/// Los ítems sin Href son secciones que todavía no tienen ruta: se muestran
/// como texto, nunca como enlaces falsos.
/// </summary>
public static class WalletAdminNavigation
{
    /// <summary>Secciones de la barra admin, en su orden de aparición.</summary>
    public static readonly IReadOnlyList<WalletNavigationItem> Items =
    [
        new("Inicio", IconName.Home, IsCurrent: true),
        new("Usuarios", IconName.Users),
        new("Transacciones", IconName.Transfer),
        new("Inversiones", IconName.Analytics),
        new("Auditorías", IconName.Audit),
    ];
}
