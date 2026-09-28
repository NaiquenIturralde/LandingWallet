using System.Globalization;

namespace LandingWallet.Components.Wallet;

/// <summary>
/// Formato de presentación de valores de Wallet.
///
/// Solo da formato a valores que ya llegan resueltos: no calcula saldos, no
/// convierte monedas y no suma importes de distintas monedas. El formato es
/// siempre argentino (separador de miles con punto, decimales con coma),
/// independientemente de la cultura del servidor.
/// </summary>
public static class WalletFormat
{
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("es-AR");

    /// <summary>
    /// Importe con su moneda: <c>$ 3.608.995,58</c> para ARS y
    /// <c>USD 100.000,00</c> para el resto, donde el código es el prefijo.
    /// </summary>
    public static string Money(decimal amount, string currencyCode)
    {
        var value = amount.ToString("N2", Culture);
        var prefix = currencyCode == "ARS" ? "$" : currencyCode;

        return $"{prefix} {value}";
    }

    /// <summary>Cantidad entera agrupada: <c>4.750</c>.</summary>
    public static string Count(int value) => value.ToString("N0", Culture);

    /// <summary>
    /// Importe con su signo explícito y su moneda: <c>+ $ 2.226.375,11</c> o
    /// <c>− $ 2.226.375,11</c>. Se usa en resultados de período, donde el signo
    /// forma parte del dato y no puede omitirse.
    ///
    /// El signo es un refuerzo: quien lo consuma debe acompañarlo con texto o
    /// iconografía, nunca depender solo de él ni del color.
    /// </summary>
    public static string MoneySigned(decimal amount, string currencyCode)
    {
        // Se usa el signo menos tipográfico (−), igual que los movimientos.
        var sign = amount < 0 ? "−" : "+";

        return $"{sign} {Money(Math.Abs(amount), currencyCode)}";
    }

    /// <summary>Fecha corta argentina: <c>17/09/2026</c>.</summary>
    public static string Date(DateOnly date) => date.ToString("dd/MM/yyyy", Culture);

    /// <summary>Tasa expresada como porcentaje: <c>5 %</c>.</summary>
    public static string Rate(decimal fraction) => $"{Format(fraction * 100)} %";

    /// <summary>
    /// Participación de <paramref name="part"/> sobre <paramref name="total"/>.
    /// Es una proporción aritmética de dos valores recibidos: no es un dato
    /// del dominio.
    /// </summary>
    public static string Share(decimal part, decimal total)
        => total == 0
            ? "0,0 %"
            : $"{FormatShare(part / total * 100)} %";

    /// <summary>
    /// Ancho relativo para una visualización de datos (0–100), en formato
    /// invariante porque se usa dentro de un estilo CSS.
    /// </summary>
    public static string ShareStyle(decimal part, decimal total)
        => total == 0
            ? "0%"
            : $"{(part / total * 100).ToString("0.##", CultureInfo.InvariantCulture)}%";

    private static string Format(decimal value) => value.ToString("0.#", Culture);

    // Las participaciones se muestran con un decimal fijo, que es más legible
    // en una distribución con categorías de peso muy distinto.
    private static string FormatShare(decimal value) => value.ToString("0.0", Culture);
}
