namespace LandingWallet.Components.Shared;

/// <summary>
/// Dirección de la tendencia de una métrica en <see cref="WalletMetricCard"/>.
/// Es solo una dirección visual: no implica cálculo ni interpretación financiera.
/// </summary>
public enum MetricTrend
{
    /// <summary>Sin tendencia definida. Es el valor por defecto.</summary>
    Neutral,

    /// <summary>Tendencia ascendente.</summary>
    Up,

    /// <summary>Tendencia descendente.</summary>
    Down,
}
