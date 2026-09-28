---
description: "Reglas del Design System de Wallet Squad 1 para componentes Razor y CSS scoped. Aplica la paleta DARK FINTECH, tokens, tipografías, Material Icons, mobile-first, accesibilidad y estados de interfaz."
applyTo: "**/*.razor,**/*.razor.css,**/*.css"
---

# Wallet Squad 1 — Design System (Razor + CSS)

Estas reglas son obligatorias para todo componente `.razor`, su CSS scoped (`.razor.css`) y cualquier hoja `.css` del proyecto. La identidad visual es **DARK FINTECH** y no se negocia.

## 1. Tokens: fuente única de verdad

- Usa **siempre** las variables `--ws-*` definidas en `wwwroot/css/tokens.css`. Nunca escribas colores, tamaños, radios, sombras o duraciones en hexadecimal/valor literal dentro de un componente.
- Si falta un token, agrégalo en `tokens.css` (con comentario) en lugar de hardcodear el valor en el componente.
- Tokens de color principales: `--ws-background`, `--ws-surface`, `--ws-card`, `--ws-cyan`, `--ws-blue`, `--ws-purple`, `--ws-white`, `--ws-muted`.
- Tokens de texto: `--ws-text`, `--ws-text-muted`, `--ws-text-accent`, `--ws-text-inverse`.
- Estados: `--ws-success`, `--ws-danger`, `--ws-warning`, `--ws-neutral` y sus variantes `-soft` / `-border`.
- Espaciado: `--ws-space-1..32`. Radios: `--ws-radius-xs..pill`. Sombras: `--ws-shadow-sm..xl`. Movimiento: `--ws-dur-fast|base|slow`, `--ws-ease-out|in-out`. Capas: `--ws-z-base|sticky|header|overlay|modal`.
- Tipografía: `--ws-font-display` (Montserrat), `--ws-font-body` (Inter), `--ws-fs-xs..5xl`, `--ws-weight-*`, `--ws-lh-*`, `--ws-ls-*`.

## 2. Paleta y color

- No introduzcas colores de marca nuevos, no uses fondos blancos ni modo claro, no uses colores aleatorios por pantalla.
- Cyan, azul y violeta son **acentos estratégicos**, no rellenos masivos. El contenido financiero es el protagonista.
- Nota de contraste (ya documentada en `tokens.css`): `--ws-blue` (~4.36:1) y `--ws-purple` (~3.67:1) **no** se usan en texto de cuerpo ni enlaces pequeños.
- Para estados, no dependas solo del color: combina icono + texto + borde + contraste (ver `StatusBadge`).

## 3. Tipografía

- Títulos, encabezados, métricas y cifras destacadas → `--ws-font-display` (Montserrat SemiBold).
- Texto general, botones, formularios, filtros, tablas y datos → `--ws-font-body` (Inter).
- Respeta la jerarquía de encabezados: `h1` solo en el Hero, `h2` para secciones (`SectionHeading`), `h3` para tarjetas (`FeatureCard`, `StepCard`). No saltees niveles.

## 4. Iconografía

- **Google Material Symbols Outlined** es el sistema oficial de iconografía, cargado centralmente desde Google Fonts en `App.razor`.
- **Única API pública: `Icon` + `IconName`** (`Components/Shared/Icon.razor`). Los consumidores usan `<Icon Name="IconName.Home" />` y **nunca** escriben `material-symbols` directamente.
- Prohibido: SVG propios como iconos funcionales, Font Awesome, Bootstrap Icons, emojis funcionales o librerías mezcladas.
- Iconos decorativos: `Decorative="true"` (por defecto). Iconos informativos: `Decorative="false"` + `AriaLabel` **obligatorio** (lanza excepción si falta).
- Si necesitas un icono nuevo, agrégalo al enum `IconName` y al mapeo de `Icon` **solo cuando exista un caso real**; no insertes SVG suelto ni spans de Material Symbols en la sección.

## 5. Estructura de secciones

- Cada sección vive en `Components/Sections/<Nombre>/<Nombre>Section.razor` + `.razor.css`, con `@namespace LandingWallet.Components.Sections`.
- Envuelve el contenido en `SectionShell` (`Id`, `HeadingId`, `CssClass`) y titula con `SectionHeading` (`Eyebrow`, `Title`, `Description`, `HeadingId`).
- `Home.razor` es **solo composición**: no agregues contenido ni estilos de sección ahí.
- Reutiliza los componentes de `Components/Shared/` antes de crear HTML nuevo: `WalletButton`, `WalletIconButton`, `WalletCard`, `WalletMetricCard`, `WalletAlert`, `WalletEmptyState`, `WalletErrorState`, `WalletSpinner`, `WalletSkeleton`, `WalletFormField`, `WalletInput`, `WalletSelect`, `StatusBadge`, `Icon`, y los de Landing (`FeatureCard`, `StepCard`, `GradientButton`, `FaqItem`, `MetricPlaceholder`, `MockupPlaceholder`).
- No crees un componente si solo agrega complejidad; tampoco repitas la misma estructura decenas de veces.

## 6. CSS scoped y convenciones

- Un archivo `.razor.css` por componente, con la misma convención BEM: `.bloque`, `.bloque__elemento`, `.bloque--modificador`.
- Usa utilidades globales de `wwwroot/css/utilities.css` cuando apliquen: `ws-container`, `ws-container--narrow`, `ws-flow`, `ws-grid`/`ws-grid--2|3|4`, `ws-eyebrow`, `ws-text-lead`, `ws-text-muted`, `ws-text-balance`, `ws-sr-only`, `ws-card`, `ws-surface`, `ws-gradient-text`, `ws-gradient-border`.
- No dupliques estilos ya cubiertos por utilidades o tokens. No dejes CSS sin usar.
- El CSS scoped de Blazor **no alcanza** a elementos renderizados por componentes hijos: si necesitas estilizar un wrapper alrededor de un hijo, ese wrapper debe pertenecer al componente que lo declara (ver `GradientButton`, `StatusBadge`).

## 7. Mobile-first y responsive

- Escribe primero el layout de una columna y agrega `@media (min-width: …)` hacia arriba. Breakpoints aprobados: 430, 768, 1024, 1200, 1440 px.
- Una vista móvil no es una versión comprimida del desktop: reorganiza cards, métricas, formularios, tablas, acciones, navegación y filtros.
- Evita scroll horizontal salvo razón justificada. Áreas interactivas ≥ 44x44 px.
- Prefiere `clamp()` y las escalas fluidas de tokens antes que media queries para tipografía y espaciado.

## 8. Accesibilidad (WCAG)

- HTML semántico (`section`, `article`, `nav`, `button`, `h1..h6`).
- Foco visible con `--ws-focus-ring` / `--ws-focus-offset`; nunca elimines el outline sin reemplazo.
- `aria-label` en controles interactivos sin texto visible; `aria-hidden="true"` en decorativos.
- Asocia encabezados con `aria-labelledby` (lo hace `SectionShell` vía `HeadingId`).
- No comuniques información solo con color. Usa `ws-sr-only` para texto solo de lectores de pantalla.

## 9. Estados de interfaz

- Toda vista que consuma datos debe contemplar cuando corresponda: Loading, Empty, Error, Success, Disabled, Processing, sin resultados, sin conexión.
- Nunca dejes una interfaz vacía porque no hay registros: crea empty states útiles.
- Mientras un dato no esté confirmado, usa placeholders honestos (`MetricPlaceholder`, `MockupPlaceholder`) en lugar de inventar valores.

## 10. Formularios

- Usa `WalletFormField<TValue>` + `WalletInput`/`WalletSelect` (derivan de `InputBase`): integración real con `EditForm`/`EditContext`/`ValidationMessage`, `aria-invalid` y `aria-describedby`.
- Label visible siempre (el placeholder no reemplaza al label), validación, mensajes comprensibles, estados focus/disabled y feedback de procesamiento.
- `WalletInput` es **solo texto**: no usar para dinero, importes, `int`, `decimal` ni cantidades financieras (`WalletNumberInput` sigue postergado).

## 11. Reglas financieras

- El **saldo no es un valor estático**: se deriva de los movimientos/transacciones. No crees UI que lo contradiga.
- Las operaciones sensibles muestran cuenta origen, destino, moneda, importe, cotización/comisión si corresponde, monto final, fecha y estado, con una instancia clara de revisión antes de confirmar.

## 12. Calidad

- Sin código muerto, sin TODOs innecesarios, sin métodos vacíos, sin eventos sin implementar, sin variables sin usar, sin componentes duplicados.
- Nombres consistentes y en español para el dominio; clases CSS en BEM con prefijo del bloque.
