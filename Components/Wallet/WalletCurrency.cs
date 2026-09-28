namespace LandingWallet.Components.Wallet;

/// <summary>
/// Monedas permitidas en la App Wallet.
///
/// ALLOWLIST EXPLÍCITA: solo ARS y USD. Ninguna otra moneda puede llegar a una
/// consulta de BigQuery: se valida antes de construir el SQL. No existe
/// conversión entre monedas: cada una se informa con sus propios datos reales.
/// </summary>
public static class WalletCurrency
{
    /// <summary>Peso argentino.</summary>
    public const string Ars = "ARS";

    /// <summary>Dólar estadounidense.</summary>
    public const string Usd = "USD";

    /// <summary>Monedas permitidas, en el orden del selector.</summary>
    public static readonly IReadOnlyList<string> Allowed = [Ars, Usd];

    /// <summary>Indica si la moneda está en la allowlist.</summary>
    public static bool IsAllowed(string? currencyCode)
        => currencyCode == Ars || currencyCode == Usd;

    /// <summary>
    /// Valida la moneda contra la allowlist. Lanza si no es ARS o USD: una
    /// moneda arbitraria nunca debe llegar a una consulta de BigQuery.
    /// </summary>
    public static string RequireAllowed(string currencyCode)
        => IsAllowed(currencyCode)
            ? currencyCode
            : throw new ArgumentOutOfRangeException(
                nameof(currencyCode), currencyCode,
                "Moneda no permitida. Solo se admiten ARS y USD.");
}