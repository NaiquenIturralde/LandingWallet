namespace LandingWallet.Components.Shared;

/// <summary>Par etiqueta/valor de <see cref="WalletDetailList"/>.</summary>
/// <param name="Label">Nombre del dato.</param>
/// <param name="Value">Valor ya formateado por quien lo consume.</param>
/// <param name="Emphasized">Destaca la fila como dato principal del bloque.</param>
/// <param name="Icon">Icono opcional de la fila.</param>
public sealed record WalletDetailItem(
    string Label,
    string Value,
    bool Emphasized = false,
    IconName? Icon = null);
