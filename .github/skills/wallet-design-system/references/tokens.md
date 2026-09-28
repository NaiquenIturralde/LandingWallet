# Tokens y sistema visual

Fuente única de verdad: `wwwroot/css/tokens.css`. Nunca hardcodear valores en componentes.

## Color — primitivos

| Token             | Valor     |
| ----------------- | --------- |
| `--ws-background` | `#050816` |
| `--ws-surface`    | `#07152E` |
| `--ws-card`       | `#0B1735` |
| `--ws-cyan`       | `#12D9FF` |
| `--ws-blue`       | `#176BFF` |
| `--ws-purple`     | `#763CFF` |
| `--ws-white`      | `#F8FAFF` |
| `--ws-muted`      | `#A9B7CE` |

## Color — derivados

`--ws-surface-raised`, `--ws-card-hover`, `--ws-border`, `--ws-border-strong`, `--ws-border-cyan`, `--ws-cyan-soft`, `--ws-grid-line`, `--ws-card-translucent`, `--ws-surface-translucent`, `--ws-overlay`, `--ws-scrim`.

## Color — texto

`--ws-text`, `--ws-text-muted`, `--ws-text-accent`, `--ws-text-inverse`.

## Color — estados

`--ws-success`, `--ws-danger`, `--ws-warning`, `--ws-neutral` + variantes `-soft` y `-border`.

## Gradientes

`--ws-gradient-main`, `--ws-gradient-main-reverse`, `--ws-gradient-text`, `--ws-gradient-surface`, `--ws-gradient-border`, `--ws-glow-cyan|blue|purple`.

## Tipografía

- `--ws-font-display` (Montserrat), `--ws-font-body` (Inter), `--ws-font-mono`.
- Pesos: `--ws-weight-regular|medium|semibold|bold`.
- Escala fluida: `--ws-fs-xs..5xl` (con `clamp()`).
- Interlineado: `--ws-lh-tight|snug|normal|relaxed`. Tracking: `--ws-ls-tight|normal|wide`.

## Espaciado, radios, sombras

- Espaciado (base 4px): `--ws-space-1..32`.
- Radios: `--ws-radius-xs|sm|md|lg|xl|pill`.
- Sombras: `--ws-shadow-sm|md|lg|xl|cyan`.

## Layout, movimiento, capas, foco

- Layout: `--ws-container-max`, `--ws-container-narrow`, `--ws-container-pad`, `--ws-header-h`, `--ws-section-py`, `--ws-section-gap`.
- Movimiento: `--ws-dur-fast|base|slow`, `--ws-ease-out|in-out`.
- Capas: `--ws-z-base|sticky|header|overlay|modal`.
- Foco: `--ws-focus-ring`, `--ws-focus-offset`.
- Bordes: `--ws-hairline`.

## Contraste (documentado en tokens.css)

- `--ws-white` ≈ 19:1, `--ws-muted` ≈ 8.7:1, `--ws-cyan` ≈ 11.8:1 → aptos para todo.
- `--ws-blue` ≈ 4.36:1 y `--ws-purple` ≈ 3.67:1 → **no** usar en texto de cuerpo ni enlaces pequeños.

## Utilidades globales (`utilities.css`)

`ws-container`, `ws-container--narrow`, `ws-flow`, `ws-flow--lg|xl`, `ws-grid`, `ws-grid--2|3|4`, `ws-gradient-text`, `ws-eyebrow`, `ws-text-muted`, `ws-text-lead`, `ws-text-balance`, `ws-sr-only`, `ws-skip-link`, `ws-surface`, `ws-card`, `ws-gradient-border`.
