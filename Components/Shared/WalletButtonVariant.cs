namespace LandingWallet.Components.Shared;

/// <summary>
/// Variantes visuales de <see cref="WalletButton"/>.
/// Es un enum propio del botón funcional: no se comparte con
/// <see cref="ButtonVariant"/>, que describe el enlace CTA <see cref="GradientButton"/>.
/// </summary>
public enum WalletButtonVariant
{
    /// <summary>Acción principal. Gradiente cyan → blue → purple.</summary>
    Primary,

    /// <summary>Acción secundaria. Superficie navy con borde sutil.</summary>
    Secondary,

    /// <summary>Acción terciaria. Sin fondo ni borde hasta el hover.</summary>
    Ghost,

    /// <summary>Acción destructiva o irreversible. Requiere confirmación en el flujo.</summary>
    Danger,
}
