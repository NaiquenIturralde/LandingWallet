using LandingWallet.Components;
using LandingWallet.Components.Wallet;
using LandingWallet.Components.Wallet.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ---------------------------------------------------------------------------
// BigQuery — configuración y capa de acceso a datos.
//
// La configuración (ProjectId, datasets, usuario demo) vive en appsettings.json
// (sección WalletBigQuery). Las CREDENCIALES NO viven en el repositorio: el
// cliente usa Application Default Credentials (variable de entorno
// GOOGLE_APPLICATION_CREDENTIALS, cuenta de servicio adjunta al entorno o
// `gcloud auth application-default login`).
//
// El cliente NO se registra acá: la capa de acceso lo crea de forma perezosa en
// la primera consulta, para que la ausencia de credenciales se resuelva como
// ERROR en la pantalla (dentro del try/catch del componente) y no como una
// excepción de arranque.
// ---------------------------------------------------------------------------
builder.Services.Configure<WalletBigQueryOptions>(
    builder.Configuration.GetSection(WalletBigQueryOptions.SectionName));

// Capa ÚNICA de acceso a BigQuery (SQL y lectura de filas).
builder.Services.AddScoped<IWalletBigQueryDataSource, WalletBigQueryDataSource>();

// App Wallet (rol usuario y admin). Acceso a datos aislado tras la interfaz:
// las pantallas dependen solo del contrato, nunca de BigQuery.
builder.Services.AddScoped<IWalletDashboardService, WalletDashboardService>();

// Reportes de Looker Studio: la App Wallet consume el catálogo estático
// WalletLookerReports (URLs públicas, sin secretos) — no hay configuración
// de embed monolítico: cada reporte vive en su WalletLookerCard.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
