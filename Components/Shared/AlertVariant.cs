namespace LandingWallet.Components.Shared;

/// <summary>
/// Variantes semánticas de <see cref="WalletAlert"/>.
/// Cada variante define su icono y su tratamiento visual: el color nunca es el
/// único indicador del significado.
/// </summary>
public enum AlertVariant
{
    /// <summary>Información contextual, sin carga de éxito ni de falla.</summary>
    Info,

    /// <summary>Confirmación de una acción completada correctamente.</summary>
    Success,

    /// <summary>Advertencia que conviene revisar antes de continuar.</summary>
    Warning,

    /// <summary>Error que impide completar una acción.</summary>
    Error,
}
