namespace LandingWallet.Components.Wallet;

/// <summary>
/// Configuración del acceso a BigQuery para la App Wallet.
///
/// ÚNICA FUENTE DE LA SELECCIÓN DEMO: <c>appsettings.json</c> (sección
/// <see cref="SectionName"/>). El <c>id_usuario</c> y el nombre visible NO se
/// dispersan por los componentes: se leen de acá.
///
/// SEGURIDAD: acá NO viven credenciales. El <c>BigQueryClient</c> se autentica
/// con Application Default Credentials (variable de entorno
/// <c>GOOGLE_APPLICATION_CREDENTIALS</c>, cuenta de servicio adjunta o
/// <c>gcloud auth application-default login</c>). El repositorio nunca
/// contiene secretos.
/// </summary>
public sealed class WalletBigQueryOptions
{
    /// <summary>Nombre de la sección de configuración.</summary>
    public const string SectionName = "WalletBigQuery";

    /// <summary>Project ID de Google Cloud donde viven los datasets.</summary>
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>Dataset de mart (vistas de reporting del proyecto).</summary>
    public string MartDataset { get; set; } = "wallet_mart";

    /// <summary>Dataset core (dimensiones y hechos del dominio).</summary>
    public string CoreDataset { get; set; } = "wallet_core";

    /// <summary>
    /// Usuario demo seleccionado para la presentación. Es el
    /// <c>id_usuario</c> con el que se filtran las vistas de usuario.
    /// </summary>
    public int DemoUserId { get; set; }

    /// <summary>
    /// Nombre visible de la demo. La fuente real almacena otro nombre; se
    /// sobrescribe SOLO el rótulo visible y se deja constancia explícita en la
    /// interfaz para no presentarlo como un dato de la fuente.
    /// </summary>
    public string DemoDisplayName { get; set; } = string.Empty;

    /// <summary>Indica si la configuración mínima está presente.</summary>
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(ProjectId)
        && !string.IsNullOrWhiteSpace(MartDataset)
        && !string.IsNullOrWhiteSpace(CoreDataset)
        && DemoUserId > 0;
}
