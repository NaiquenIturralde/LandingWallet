namespace LandingWallet.Components.Shared;

/// <summary>
/// Contexto interno del Design System que <see cref="WalletFormField"/> publica
/// por cascada para que los controles hijos (<see cref="WalletInput"/>,
/// <see cref="WalletSelect{TValue}"/>) resuelvan sus ids y su accesibilidad sin
/// que el consumidor tenga que repetir parámetros.
/// No es un servicio ni un estado global: vive y muere con el campo.
/// </summary>
internal sealed class FormFieldContext
{
    /// <summary>Id del control real (input/select).</summary>
    public required string ControlId { get; init; }

    /// <summary>Id del hint, si el campo tiene uno.</summary>
    public string? HintId { get; init; }

    /// <summary>Id del mensaje de validación, si el campo tiene uno.</summary>
    public string? ErrorId { get; init; }

    /// <summary>
    /// Valor listo para el atributo aria-describedby: hint y error separados por
    /// espacio. Es null cuando no hay ninguno de los dos.
    /// </summary>
    public string? DescribedBy { get; init; }

    /// <summary>Indica si el campo es obligatorio.</summary>
    public bool Required { get; init; }

    /// <summary>Indica si el campo está deshabilitado.</summary>
    public bool Disabled { get; init; }
}
