using LandingWallet.Components.Shared;

namespace LandingWallet.Components.Wallet;

/// <summary>
/// Ítem de la navegación de la App Wallet (sidebar desktop y barra inferior mobile).
/// </summary>
/// <param name="Label">Texto visible del ítem.</param>
/// <param name="Icon">
/// Icono del ítem, siempre a través de <see cref="Icon"/>. Opcional: los
/// labels de sección (encabezados de grupo) no llevan icono.
/// </param>
/// <param name="Href">
/// Ruta de destino. Si es <c>null</c>, la sección todavía no tiene ruta: se
/// muestra como texto, nunca como enlace falso.
/// </param>
/// <param name="IsCurrent">
/// Marca manual de sección actual. Solo aplica a las secciones sin ruta: cuando
/// hay <see cref="Href"/>, la sección activa la determina la navegación.
/// </param>
/// <param name="IsSectionLabel">
/// Es un encabezado de grupo (por ejemplo "DASHBOARDS"): texto pequeño, no
/// clickeable y sin foco. Solo se muestra en la sidebar desktop.
/// </param>
/// <param name="DesktopOnly">
/// El ítem existe SOLO en desktop (sidebar). En la barra inferior mobile se
/// oculta: los accesos secundarios se alcanzan desde Más.
/// </param>
/// <param name="IsSectionChild">
/// Es miembro de contenido de un grupo (va debajo de un <see cref="IsSectionLabel"/>):
/// en desktop recibe una sangría leve para leerse como subcategoría.
/// </param>
public sealed record WalletNavigationItem(
    string Label,
    IconName? Icon = null,
    string? Href = null,
    bool IsCurrent = false,
    bool IsSectionLabel = false,
    bool DesktopOnly = false,
    bool IsSectionChild = false)
{
    /// <summary>Indica si el ítem puede comportarse como enlace.</summary>
    public bool IsLink => !string.IsNullOrWhiteSpace(Href);
}
