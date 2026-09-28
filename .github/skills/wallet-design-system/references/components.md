# Componentes del Design System

Ubicación: `Components/Shared/`. Reutilizar antes de crear.

## Acciones

### WalletButton

Botón funcional `<button>` con texto visible.

| Parámetro     | Tipo                  | Default                                    |
| ------------- | --------------------- | ------------------------------------------ |
| `Text`        | `string`              | _(obligatorio — lanza excepción si vacío)_ |
| `Variant`     | `WalletButtonVariant` | `Primary`                                  |
| `Type`        | `WalletButtonType`    | `Button`                                   |
| `IconName`    | `IconName?`           | `null`                                     |
| `Disabled`    | `bool`                | `false`                                    |
| `Loading`     | `bool`                | `false`                                    |
| `LoadingText` | `string?`             | `null`                                     |
| `FullWidth`   | `bool`                | `false`                                    |
| `OnClick`     | `EventCallback`       | —                                          |

Variantes: `Primary`, `Secondary`, `Ghost`, `Danger`. En `Loading` muestra `WalletSpinner` decorativo y expone `aria-busy`. **No usar para icon-only.**

### WalletIconButton

Acción solo-icono. `AriaLabel` **obligatorio** (lanza excepción si vacío). Área táctil 44×44. `Loading` con `WalletSpinner` decorativo. Sin `LoadingText`.

### GradientButton

CTA/enlace `<a>` (`Text`, `Href`, `Variant`, `AriaLabel`, `Target`, `ShowArrow`). **No** sustituye a `WalletButton`.

## Contenedores y métricas

### WalletCard

Contenedor visual **no interactivo**: `ChildContent`, `Elevated` (true), `Padded` (true), `CssClass`.

### WalletMetricCard

`Label` + `Value` obligatorios; `Description`, `Delta`, `Trend` (`MetricTrend`), `IconName` opcionales. Sin lógica financiera.

## Loading

### WalletSpinner

`Size` (24), `Label`, `Decorative` (false). Informativo → `Label` obligatorio + `role="status"`. Decorativo → `aria-hidden`. No define `aria-busy`.

### WalletSkeleton

`Shape` (`SkeletonShape`: `Line`/`Block`/`Circle`), `Width`, `Height`, `Lines`. `aria-hidden`, respeta `prefers-reduced-motion`.

## Feedback y estados

### WalletAlert

`Variant` (`AlertVariant`: `Info`/`Success`/`Warning`/`Error`), `Title`, `Text` (obligatorio), `ChildContent`. Info/Success → `role="status"`; Warning/Error → `role="alert"`.

### WalletEmptyState

`Title` (obligatorio), `Description`, `IconName`, `ActionText`, `OnAction`. **Sin `role="alert"`.** Acción vía `WalletButton`.

### WalletErrorState

`Title` (obligatorio), `Description`, `IconName` (default `Error`), `RetryText`, `OnRetry`. `role="alert"`. Retry vía `WalletButton`.

### StatusBadge

`Text` (obligatorio), `Variant` (`StatusVariant`: `Success`/`Error`/`Pending`/`Neutral`). Icono + texto.

## Iconografía

### Icon

`Name` (`IconName`, obligatorio), `Size` (24), `Decorative` (true), `AriaLabel`. Informativo exige `AriaLabel`.

## Landing (reutilización contextual)

`SectionShell` (`Id`, `CssClass`, `HeadingId`, `ChildContent`), `SectionHeading` (`Eyebrow`, `Title`, `Description`, `Align`, `HeadingId`), `FeatureCard` (`IconName`, `Title`, `Description`), `StepCard` (`Number`, `Title`, `Description`, `IconName?`, `Note?`), `FaqItem` (`Question`, `Answer`), `MetricPlaceholder` (`Label?`), `MockupPlaceholder` (`Title`, `Description?`, `AspectRatio`).

## Enums

`IconName`, `WalletButtonVariant`, `WalletButtonType`, `MetricTrend`, `AlertVariant`, `SkeletonShape`, `ButtonVariant`, `StatusVariant`, `HeadingAlign`, `MockupAspectRatio`.
