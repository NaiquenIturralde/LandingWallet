---
description: "Genera una nueva sección de la landing Wallet Squad 1 (.razor + .razor.css) respetando la estructura existente, componentes reutilizables, mobile-first, Material Icons, accesibilidad y la paleta oficial."
agent: "Wallet Squad Architect"
argument-hint: "Nombre de la sección y su propósito (ej: 'Recompensas — explica el programa de puntos')"
---

# Nueva sección de la landing

Crea una nueva sección de la landing de Wallet Squad 1 siguiendo exactamente las convenciones del proyecto y el Design System.

## Entrada

- **Nombre de la sección**: ${input:nombre:Nombre de la sección (PascalCase, ej: Rewards)}
- **Propósito / contenido**: ${input:proposito:Qué debe comunicar la sección y qué datos del modelo funcional usa}
- **Posición en la página**: ${input:posicion:Dónde va dentro de Home.razor (antes/después de qué sección)}

## Pasos

1. **Analiza** primero una sección existente como referencia (`Components/Sections/About/AboutSection.razor` y su `.razor.css`) y revisa `Components/Shared/` para reutilizar componentes antes de escribir HTML nuevo.
2. **Crea** `Components/Sections/<Nombre>/<Nombre>Section.razor` con:
   - `@namespace LandingWallet.Components.Sections`
   - Comentario de cabecera describiendo la sección (número, propósito, origen de los datos).
   - Envoltura con `SectionShell` (`Id` en kebab-case, `HeadingId`, `CssClass`).
   - Título con `SectionHeading` (`Eyebrow`, `Title`, `Description`, `HeadingId`).
   - Contenido con componentes de `Components/Shared/` (`FeatureCard`, `StepCard`, `StatusBadge`, `GradientButton`, `FaqItem`, `MetricPlaceholder`, `MockupPlaceholder`, `Icon`).
   - Para acciones funcionales usar `WalletButton` (no `GradientButton`, que es un enlace CTA).
   - Para estados usar `WalletEmptyState`/`WalletErrorState`/`WalletAlert`/`WalletSkeleton` cuando corresponda.
   - Jerarquía correcta: `h2` en el heading de sección, `h3` en las tarjetas.
3. **Crea** `Components/Sections/<Nombre>/<Nombre>Section.razor.css` con:
   - Comentario de cabecera indicando el comportamiento responsive (ej: "1 columna → 2 (tablet) → 4 (desktop)").
   - Convención BEM (`.bloque`, `.bloque__elemento`, `.bloque--modificador`).
   - Solo tokens `--ws-*`; sin valores literales de color/tamaño.
   - Mobile-first: base de 1 columna y `@media (min-width: …)` hacia arriba.
4. **Registra** la sección en `Components/Pages/Home.razor` en la posición indicada (solo la etiqueta del componente, sin contenido ni estilos).
5. **Verifica** que compile (`dotnet build`) y corrige cualquier error o warning evitable.

## Reglas obligatorias

- Paleta DARK FINTECH con tokens `--ws-*`; sin colores nuevos, sin fondos blancos, sin modo claro.
- Montserrat para títulos/métricas, Inter para texto e interfaz.
- Iconos solo vía `Icon` + `IconName` (Google Material Symbols Outlined); nada de SVG propios, emojis ni librerías externas.
- Mobile-first y táctil (áreas ≥ 44x44 px), sin scroll horizontal.
- Accesibilidad: HTML semántico, foco visible, `aria-label`/`aria-hidden` correctos, sin depender solo del color.
- Estados de interfaz cuando corresponda (loading/empty/error/success/disabled/processing).
- No inventes datos: usa `MetricPlaceholder`/`MockupPlaceholder` si el dato no está confirmado.
- El saldo se deriva de movimientos; no lo trates como valor estático.
- No agregues dependencias, no dupliques CSS ni componentes, no dejes código muerto.

## Salida esperada

1. Archivos creados/modificados con sus rutas.
2. Resultado de la compilación.
3. Checklist de validación final (paleta, tipografía, iconos, mobile, desktop, estados, accesibilidad, reutilización, modelo de datos).
