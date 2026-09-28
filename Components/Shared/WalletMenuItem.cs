namespace LandingWallet.Components.Shared;

/// <summary>Ítem de <see cref="WalletMenuList"/>.</summary>
/// <param name="Label">Texto principal del ítem.</param>
/// <param name="Icon">Icono, siempre a través de <see cref="Icon"/>.</param>
/// <param name="Description">Detalle opcional.</param>
/// <param name="Href">
/// Destino. Si es <c>null</c>, la sección todavía no está habilitada y el ítem
/// se muestra como estado pendiente, nunca como enlace falso.
/// </param>
/// <param name="PendingLabel">Texto del estado pendiente. Por defecto "Próximamente".</param>
public sealed record WalletMenuItem(
    string Label,
    IconName Icon,
    string? Description = null,
    string? Href = null,
    string? PendingLabel = null)
{
    /// <summary>Indica si el ítem puede comportarse como enlace.</summary>
    public bool IsLink => !string.IsNullOrWhiteSpace(Href);
}
