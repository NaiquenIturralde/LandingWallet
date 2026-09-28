namespace LandingWallet.Components.Shared;

/// <summary>
/// Estados de <see cref="StatusBadge"/>.
/// Se corresponden con los resultados definidos en el modelo funcional
/// (una transacción puede resultar aprobada o rechazada) más los estados
/// genéricos de interfaz.
/// </summary>
public enum StatusVariant
{
    /// <summary>Aprobada / activa.</summary>
    Success,

    /// <summary>Rechazada / con error.</summary>
    Error,

    /// <summary>En proceso, sin resultado todavía.</summary>
    Pending,

    /// <summary>Informativo, sin carga semántica de éxito o falla.</summary>
    Neutral,
}
