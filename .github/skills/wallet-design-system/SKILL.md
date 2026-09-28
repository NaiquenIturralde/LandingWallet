---
name: wallet-design-system
description: "Design System oficial de Wallet Squad 1 (Blazor/.NET, DARK FINTECH). USE FOR: construir o modificar UI de Wallet Squad 1; elegir qué componente reutilizar; crear pantallas, secciones, formularios o estados; aplicar tokens, paleta, tipografía, Material Symbols, accesibilidad, responsive mobile-first; saber qué componentes existen y cuáles siguen postergados; respetar las reglas de datos financieros. Trigger words: Wallet Squad, wallet, fintech, Blazor, Razor, Design System, dark fintech, tokens, componente, formulario, accesibilidad, responsive, Material Symbols, Maximiliano."
argument-hint: "Describe la pantalla, componente o flujo Wallet a construir o revisar..."
---

# Wallet Squad 1 — Design System

Design System oficial y reutilizable de Wallet Squad 1. Úsalo **antes** de crear cualquier componente o pantalla: primero reutiliza lo que existe.

## Cuándo usar este Skill

- Vas a construir o modificar UI de Wallet Squad 1 (landing, App Wallet Usuario o Admin).
- Necesitás saber qué componente reutilizar y cómo usarlo.
- Necesitás confirmar qué componentes **no** existen todavía.
- Necesitás aplicar tokens, tipografía, iconografía, accesibilidad o responsive.

## Fundamentos

- **.NET 10**, **Blazor Web App**, **InteractiveServer**, Razor Components, C#.
- **Mobile-first**, responsive.
- Identidad visual **DARK FINTECH** (obligatoria).
- JavaScript solo cuando Blazor no pueda resolver el comportamiento.
- No agregar paquetes NuGet, frameworks CSS ni librerías JS sin necesidad técnica real.

## Paleta oficial (obligatoria)

| Rol | Valor | Token |
|---|---|---|
| Background | `#050816` | `--ws-background` |
| Surface | `#07152E` | `--ws-surface` |
| Cards | `#0B1735` | `--ws-card` |
| Cyan | `#12D9FF` | `--ws-cyan` |
| Blue | `#176BFF` | `--ws-blue` |
| Purple | `#763CFF` | `--ws-purple` |
| Texto principal | `#F8FAFF` | `--ws-white` / `--ws-text` |
| Texto secundario | `#A9B7CE` | `--ws-muted` / `--ws-text-muted` |

**Regla:** usar siempre tokens `--ws-*`. **Nunca** hardcodear colores en componentes. No introducir colores de marca nuevos, fondos blancos ni modo claro.

## Tipografía

- **Montserrat SemiBold/Bold** (`--ws-font-display`): títulos, métricas, cifras destacadas.
- **Inter** (`--ws-font-body`): cuerpo, botones, formularios, labels, controles, mensajes.

## Iconografía

- **Google Material Symbols Outlined** es el sistema oficial (cargado centralmente desde Google Fonts en `App.razor`).
- **Única API pública: `Icon` + `IconName`.** Los consumidores nunca escriben `material-symbols` directamente.
- Prohibido: SVG propios como iconos funcionales, Font Awesome, Bootstrap Icons, emojis funcionales.
- Iconos decorativos → `Decorative="true"` (default). Iconos informativos → `Decorative="false"` + `AriaLabel` **obligatorio** (lanza excepción si falta).
- Agregar un `IconName` nuevo **solo cuando exista un caso real**.

## Componentes disponibles

### Acciones
- **`WalletButton`** — botón funcional `<button>` con texto visible. Variantes: `Primary`, `Secondary`, `Ghost`, `Danger`. Soporta `IconName` opcional, `Loading` + `LoadingText` (con `WalletSpinner` interno), `Type` (`Button`/`Submit`/`Reset`), `Disabled`, `aria-busy`. **`Text` es obligatorio** (lanza excepción si está vacío). **No usar para icon-only.**
- **`WalletIconButton`** — acción solo-icono. **`AriaLabel` obligatorio.** Área táctil 44×44. `Loading` con `WalletSpinner` decorativo (sin `LoadingText`). Icono y spinner nunca simultáneos.
- **`GradientButton`** — CTA/enlace `<a>`. **No** sustituye a `WalletButton` funcional.

### Contenedores y métricas
- **`WalletCard`** — contenedor visual **no interactivo** (`Elevated`, `Padded`, `CssClass`). Sin navegación ni lógica.
- **`WalletMetricCard`** — métrica genérica: `Label` + `Value` obligatorios; `Description`, `Delta`, `Trend` (`MetricTrend`: `Up`/`Down`/`Neutral`), `IconName` opcionales. **Sin lógica financiera.**

### Formularios
- **`WalletLabel`** — `<label>` real (`Text`, `For`, `Required`).
- **`WalletFormField<TValue>`** — agrupa label + control + hint + error. Publica `FormFieldContext` por cascada (ids + `aria-describedby`). `For` es `Expression<Func<TValue>>`.
- **`WalletInput`** — `InputBase<string?>`. **Solo texto.**
- **`WalletSelect<TValue>`** — `InputBase<TValue>`, opciones por `ChildContent`, conversión vía `BindConverter`.
- **`FormFieldContext`** — contexto interno (no es servicio ni estado global).
- **`forms.css`** — primitivas compartidas (`.ws-field-control`).

Reglas: `EditForm`/`EditContext`/`InputBase`/`ValueExpression`/`ValidationMessage` reales; binding nativo (sin binding manual paralelo); `aria-invalid` desde `EditContext`; `aria-describedby` con `ErrorId` estable; `label`/`for`/`id`; `NameAttributeValue`; `AdditionalAttributes`.

**`WalletInput` NO se usa para** dinero, importes, `int`, `decimal` ni cantidades financieras. `WalletNumberInput` sigue postergado.

### Loading
- **`WalletSpinner`** — informativo → `Label` obligatorio + `role="status"`; decorativo → `aria-hidden`. **No define `aria-busy`.**
- **`WalletSkeleton`** — `aria-hidden`, respeta `prefers-reduced-motion`. **No define `aria-busy`.**

**Regla:** `aria-busy` pertenece al **contenedor** que está cargando.

### Feedback y estados
- **`WalletAlert`** — mensaje contextual. Info/Success → `role="status"`; Warning/Error → `role="alert"`. Sin `aria-live` redundante.
- **`WalletEmptyState`** — situación normal sin contenido. **Nunca `role="alert"`.** Acción opcional vía `WalletButton`.
- **`WalletErrorState`** — fallo de carga/proceso. `role="alert"`. Retry opcional vía `WalletButton`.
- **`StatusBadge`** — estado compacto de entidad. Icono + texto; no depender solo del color.

### Landing (reutilización contextual)
`SectionShell`, `SectionHeading`, `FeatureCard`, `StepCard`, `FaqItem`, `MetricPlaceholder`, `MockupPlaceholder`, `GradientButton`. **No asumir** que pertenecen a la App Wallet.

## Componentes postergados (NO disponibles)

`WalletNumberInput`, `WalletSearch`, `WalletFilter`, `WalletAccountCard`, `WalletTransactionItem`, `WalletModal`, `WalletDialog`, `WalletDrawer`, `WalletToast`, `WalletTabs`, `WalletBreadcrumb`, `WalletBottomNavigation`, `WalletSidebar`, `WalletTopbar`.

**Regla:** no crearlos preventivamente. Crearlos cuando aparezca el primer caso real que defina su contrato.

## Responsive

- Mobile-first. Breakpoints aprobados: **430, 768, 1024, 1200, 1440**.
- No obligar a cada componente a usar todos los breakpoints.
- Touch target mínimo ≈ **44×44 px**. Sin overflow horizontal.

## Accesibilidad (obligatoria)

Focus visible, navegación por teclado, labels reales, nombres accesibles, `aria-invalid`, `aria-describedby`, `aria-busy`, `aria-hidden`, roles correctos, `prefers-reduced-motion`, estados no comunicados solo por color. **No agregar ARIA redundante sobre semántica HTML nativa.**

## Datos y dominio

El Design System **no contiene lógica financiera**. Cadena de datos futura:

```
MOCKS → BIGQUERY → consultas/vistas/procedimientos → LOOKER STUDIO → interfaz
```

**Nunca inventar** saldos, montos, cuentas, movimientos, métricas, fechas, operaciones ni datos de usuarios.

**Saldo:** se **deriva de transacciones**. Nunca se persiste ni hardcodea solo para completar UI.

**Maximiliano** será el usuario demo oficial, pero **no inventar** apellido, email, CBU, alias, cuentas, saldo, movimientos, operaciones ni inversiones: primero deben identificarse sus registros reales en los mocks oficiales.

## CSS

- Tokens como fuente de verdad.
- CSS scoped por componente (BEM: `.bloque__elemento--modificador`).
- `forms.css` **exclusivamente** para primitivas compartidas de formularios.
- `utilities.css` solo utilidades reales.
- Evitar duplicación; respetar CSS isolation; sin `!important` salvo necesidad justificada; no depender accidentalmente del CSS scoped de otro componente.

## Checklist antes de finalizar

- [ ] Reutilicé un componente existente antes de crear otro.
- [ ] No inventé datos.
- [ ] Usé tokens `--ws-*`.
- [ ] Material Symbols pasa por `Icon`.
- [ ] Responsive mobile-first.
- [ ] Teclado y focus visible.
- [ ] Estados loading/empty/error cuando corresponda.
- [ ] Accesibilidad ARIA correcta.
- [ ] Sin CSS duplicado evidente.
- [ ] `dotnet build` limpio.

## Referencias

- [Tokens y visual](./references/tokens.md)
- [Componentes](./references/components.md)
- [Formularios](./references/forms.md)
- [Accesibilidad](./references/accessibility.md)
- [Reglas de datos](./references/data-rules.md)
