# Formularios

Integración real con el sistema de formularios de Blazor. No son inputs HTML decorativos.

## Componentes

### WalletLabel

`<label>` real: `Text` (obligatorio), `For` (id del control), `Required`. El indicador de obligatorio no depende solo del color (asterisco + texto `ws-sr-only`).

### WalletFormField<TValue>

Agrupa label + control + hint + error. Genera un `uid` y publica `FormFieldContext` por cascada.

| Parámetro               | Tipo                       | Default         |
| ----------------------- | -------------------------- | --------------- |
| `Label`                 | `string?`                  | `null`          |
| `Control`               | `RenderFragment`           | _(obligatorio)_ |
| `Hint`                  | `string?`                  | `null`          |
| `Required`              | `bool`                     | `false`         |
| `Disabled`              | `bool`                     | `false`         |
| `For`                   | `Expression<Func<TValue>>` | _(obligatorio)_ |
| `ShowValidationMessage` | `bool`                     | `true`          |

### WalletInput

`InputBase<string?>`. **Solo texto.** `InputType`, `Placeholder`, `Id`, `Name`, `Disabled`, `ExtraCssClass` + `AdditionalAttributes`.

### WalletSelect<TValue>

`InputBase<TValue>`. Opciones por `ChildContent`. Conversión vía `BindConverter` (sin switch manual). `Id`, `Name`, `Disabled`, `ExtraCssClass`.

### FormFieldContext

Contexto interno (`internal`): `ControlId`, `HintId`, `ErrorId`, `DescribedBy`, `Required`, `Disabled`. No es servicio ni estado global.

### forms.css

Primitivas compartidas: `.ws-field-control` (+ `--select`). Vive en `wwwroot/css/forms.css` porque el CSS scoped no alcanza el markup de componentes hijos.

## Reglas técnicas

- `EditForm` / `EditContext` / `FieldIdentifier` / `ValueExpression` / `@bind-Value` nativos.
- `ValidationMessage<TValue>` real (sin duplicar DataAnnotations).
- **Sin binding manual paralelo** (usa `CurrentValueAsString` de `InputBase`).
- `aria-invalid` calculado desde `EditContext.GetValidationMessages(FieldIdentifier)`, no desde CSS.
- `aria-describedby` con `ErrorId` **estable** (el contenedor del error existe siempre que `ShowValidationMessage=true`, aunque esté vacío).
- `NameAttributeValue` como fallback de `name`.
- `AdditionalAttributes` se pasan al control real.

## Combinaciones de aria-describedby

| Escenario                            | Valor                            |
| ------------------------------------ | -------------------------------- |
| Hint + validación activa             | `"ws-hint-{uid} ws-error-{uid}"` |
| Sin hint + validación activa         | `"ws-error-{uid}"`               |
| Hint + `ShowValidationMessage=false` | `"ws-hint-{uid}"`                |
| Ninguno                              | _(sin atributo)_                 |

## Uso

```razor
<WalletFormField TValue="string?" For="@(() => model.Nombre)" Label="Nombre" Required="true">
    <WalletInput @bind-Value="model.Nombre" Placeholder="Ingresá tu nombre" />
</WalletFormField>
```

## Restricciones

- `WalletInput` **no** se usa para dinero, importes, `int`, `decimal` ni cantidades financieras.
- `WalletNumberInput` sigue **postergado** hasta que exista un caso funcional real.
- `WalletSelect` no implementa selección múltiple.
