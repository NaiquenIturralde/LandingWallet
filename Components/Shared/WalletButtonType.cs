namespace LandingWallet.Components.Shared;

/// <summary>
/// Tipo nativo de un botón. Evita pasar strings HTML inválidos al atributo
/// <c>type</c> y centraliza el mapeo al valor que espera el navegador.
/// </summary>
public enum WalletButtonType
{
    /// <summary>Botón sin comportamiento por defecto. Es el valor recomendado.</summary>
    Button,

    /// <summary>Envía el formulario que lo contiene.</summary>
    Submit,

    /// <summary>Restablece el formulario que lo contiene.</summary>
    Reset,
}

/// <summary>
/// Mapeo compartido de <see cref="WalletButtonType"/> al valor del atributo
/// HTML <c>type</c>. Se centraliza acá para no duplicar la lógica entre
/// <see cref="WalletButton"/> y <see cref="WalletIconButton"/>.
/// </summary>
internal static class WalletButtonTypeExtensions
{
    /// <summary>Devuelve el valor HTML del atributo <c>type</c>.</summary>
    public static string ToHtmlValue(this WalletButtonType type) => type switch
    {
        WalletButtonType.Submit => "submit",
        WalletButtonType.Reset => "reset",
        _ => "button",
    };
}
