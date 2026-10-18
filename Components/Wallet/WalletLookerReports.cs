namespace LandingWallet.Components.Wallet;

/// <summary>
/// Reporte individual de Looker Studio para las cards modulares del dashboard.
/// Las URLs son públicas (no son secretos) y vienen tal como las generó
/// Looker Studio —incluido /page/…—; se asignan al src sin transformación.
/// Width/Height son el tamaño del CONTENIDO visible de la página en Looker
/// Studio —medido en vivo—: 320×340 en cinco reportes, 320×300 en «Ingresos y
/// egresos», 320×560 en «Distribución de egresos» y 600×300 en «Movimientos por
/// día». En esos dos últimos casos el LIENZO de la página es mayor (320×340 y
/// 600×450) pero su franja inferior está vacía (sin componentes, verificado
/// en el DOM del embed): usar el alto del contenido evita esa banda de espacio
/// muerto SIN perder ningún gráfico, tabla, etiqueta ni control. Esa misma
/// relación alimenta el viewport del card, de modo que título, descripción,
/// filtro, ejes y leyenda entran completos y Looker no muestra scroll interno
/// ni deja bandas de espacio vacío. No es un ancho físico: en desktop el
/// iframe se expande proporcionalmente.
/// NOTA: si una página tuviera contenido por debajo de su propio lienzo,
/// Looker lo recorta dentro del reporte (es un ajuste del reporte, no del
/// sitio): en ese caso hay que corregir el tamaño de página en Looker Studio.
/// </summary>
/// <param name="Icon">Icono del encabezado del card.</param>
/// <param name="Description">
/// Explicación breve y no técnica que se muestra debajo del título. Es
/// opcional: solo se renderiza cuando el reporte la define en el catálogo.
/// </param>
public sealed record WalletLookerReport(
    string Title,
    string ReportUrl,
    int Width,
    int Height,
    Shared.IconName Icon,
    string? Description = null);

/// <summary>
/// Catálogo único de los reportes individuales de Looker (fuente única de
/// verdad de títulos/URLs, al estilo de WalletUserNavigation).
///
/// ESTADO DE EMBEDS (verificado): los reportes del catálogo están habilitados
/// por el propietario. "Estado de las transacciones" (4c540c2e…/page/iVx9F)
/// estuvo bloqueado temporalmente — el donut nativo cubrió su métrica — y ya
/// fue habilitado e integrado. "Movimientos por día" (76388fd3…/page/bLgAG)
/// es el informe más reciente (lienzo 600 × 319).
/// </summary>
public static class WalletLookerReports
{
    /// <summary>Reportes del rol USUARIO, en orden de jerarquía visual.</summary>
    public static readonly IReadOnlyList<WalletLookerReport> User =
    [
        new("Ingresos y egresos",
            "https://datastudio.google.com/embed/reporting/bca31527-5a60-407b-b730-743828b73122/page/Qkx9F",
            // Lienzo 320 × 340; el contenido visible mide 320 × 300 (los 40px
            // inferiores del lienzo están vacíos, sin componentes).
            320, 300, Shared.IconName.Transfer,
            Description: "Compara el dinero que ingresa y sale durante el período seleccionado."),
        new("Actividad diaria",
            "https://datastudio.google.com/embed/reporting/e2f26d53-8108-4eca-97af-c1ae1709091a/page/Bhx9F",
            320, 340, Shared.IconName.TrendingUp),
        new("Distribución de egresos",
            "https://datastudio.google.com/embed/reporting/6e31e135-6c63-4f45-97e3-c361fbf04f2c/page/oix9F",
            // Lienzo real de la página en Looker Studio: 320 × 560.
            320, 560, Shared.IconName.Analytics,
            Description: "Muestra cómo se distribuyen los egresos según su categoría o tipo."),
        // Informe de lienzo 600 × 450 pero con contenido visible de 600 × 300
        // (medido en el embed: la franja inferior del lienzo está vacía, sin
        // componentes). El viewport usa el alto del contenido: el gráfico se
        // ve completo y sin la banda de espacio muerto de abajo.
        new("Movimientos por día",
            "https://datastudio.google.com/embed/reporting/76388fd3-f2e4-4ffb-bce8-b11ebc7025be/page/bLgAG",
            600, 300, Shared.IconName.Transactions,
            Description: "Este gráfico muestra cuántas transacciones se registraron en cada día del período seleccionado. Las barras más altas representan días con mayor actividad."),
    ];

    /// <summary>
    /// Reportes del rol ADMIN (6 de 6). Orden de composición: fila1
    /// sistema, fila2 operación, fila3 auditoría.
    /// </summary>
    public static readonly IReadOnlyList<WalletLookerReport> Admin =
    [
        new("Volumen transaccionado",
            "https://datastudio.google.com/embed/reporting/12ac99fc-2a85-4f64-b400-28b62515923c/page/rNx9F",
            320, 340, Shared.IconName.Currency),
        new("Altas de usuarios",
            "https://datastudio.google.com/embed/reporting/62bd0c11-3c70-4511-8ed4-6d1156a2e7d4/page/gUx9F",
            320, 340, Shared.IconName.Users),
        new("Transacciones por tipo",
            "https://datastudio.google.com/embed/reporting/7498c7ff-092a-4e8f-a1f1-c18b39b628fd/page/uWx9F",
            320, 340, Shared.IconName.Purchase),
        new("Errores de auditoría por tipo",
            "https://datastudio.google.com/embed/reporting/3a0def99-fb45-459a-8edf-cd2e8d12388d/page/4Xx9F",
            320, 340, Shared.IconName.Audit),
        new("Evolución de errores",
            "https://datastudio.google.com/embed/reporting/0f4f5f7f-a453-4a7f-9124-2c96618e37d0/page/6Zx9F",
            320, 340, Shared.IconName.Warning),
        new("Estado de las transacciones",
            "https://datastudio.google.com/embed/reporting/4c540c2e-64d6-4c40-a6f2-4f367afa6b9c/page/iVx9F",
            320, 340, Shared.IconName.Transfer),
    ];

    /// <summary>
    /// URL pública del reporte (sin /embed) para el enlace "Abrir reporte":
    /// /embed es el único tramo que distingue a la variante embebible.
    /// </summary>
    public static string FullReportUrl(WalletLookerReport report)
        => report.ReportUrl.Replace("/embed/", "/", StringComparison.OrdinalIgnoreCase);
}
