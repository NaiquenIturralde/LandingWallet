# Accesibilidad

Reglas obligatorias para todo componente y pantalla.

## Base

- HTML semántico (`section`, `article`, `nav`, `button`, `h1..h6`).
- Jerarquía de encabezados sin saltos: `h1` (Hero) → `h2` (sección) → `h3` (tarjeta).
- Foco visible con `--ws-focus-ring` / `--ws-focus-offset`. Nunca eliminar el outline sin reemplazo.
- Navegación completa por teclado.
- Touch target mínimo ≈ 44×44 px.

## Nombres accesibles

- `WalletIconButton`: `AriaLabel` obligatorio (lanza excepción si vacío).
- `Icon`: `Decorative="false"` exige `AriaLabel` (lanza excepción si falta).
- `WalletSpinner`: informativo exige `Label` (lanza excepción si falta).
- `WalletButton`: `Text` obligatorio (lanza excepción si vacío).

## ARIA por componente

| Componente                   | ARIA                                                           |
| ---------------------------- | -------------------------------------------------------------- |
| `WalletButton`               | `aria-busy="true"` en loading; `disabled` real                 |
| `WalletIconButton`           | `aria-label` obligatorio; `aria-busy` en loading               |
| `WalletAlert`                | Info/Success → `role="status"`; Warning/Error → `role="alert"` |
| `WalletEmptyState`           | **sin `role="alert"`**                                         |
| `WalletErrorState`           | `role="alert"`                                                 |
| `WalletSpinner`              | informativo → `role="status"`; decorativo → `aria-hidden`      |
| `WalletSkeleton`             | `aria-hidden="true"`                                           |
| `WalletInput`/`WalletSelect` | `aria-invalid`, `aria-describedby`, `required`, `disabled`     |

## Reglas

- **`aria-busy` pertenece al contenedor** que carga, no al spinner/skeleton.
- **No agregar ARIA redundante** sobre semántica HTML nativa (p. ej. `role="status"` ya implica live-region polite: no agregar `aria-live="polite"`).
- **No comunicar información solo con color**: usar icono + texto + borde + contraste.
- **No mover el foco automáticamente** ni usar JS para accesibilidad.
- Respetar `prefers-reduced-motion` en animaciones (spinner, skeleton).
- Texto solo para lectores de pantalla: `ws-sr-only`.
