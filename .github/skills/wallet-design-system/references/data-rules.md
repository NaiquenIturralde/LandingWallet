# Reglas de datos y dominio

El Design System **no contiene lógica financiera**. Solo presenta lo que recibe.

## Cadena de datos

```
MOCKS → BIGQUERY → consultas / vistas / procedimientos → LOOKER STUDIO → interfaz
```

Los valores de la interfaz deben mantener coherencia con estos datos. La interfaz puede transformar la **presentación** del dato, pero **no puede alterar el dato**.

## Nunca inventar

- saldos;
- montos;
- cuentas;
- movimientos;
- métricas;
- fechas;
- operaciones;
- datos de usuarios.

## Regla del saldo

El saldo **no** se persiste como valor estático de cuenta: **se deriva de las transacciones**. Nunca hardcodear un saldo para completar visualmente una pantalla.

## Datos que no existen

Si una pantalla necesita información que no existe en los mocks:

1. No inventarla.
2. Informar qué dato falta.
3. Indicar qué entidad/campo/relación necesitaría.
4. Esperar definición.

Para estructura visual pueden usarse placeholders explícitos (`MetricPlaceholder`, `MockupPlaceholder`), **nunca** números financieros falsos que parezcan reales.

## Maximiliano

Será el usuario demo oficial de la experiencia Usuario. **No inventar** apellido, email, CBU, alias, cuentas, saldo, movimientos, operaciones ni inversiones. Primero deben identificarse sus registros reales en los mocks oficiales.

## Roles

- **Usuario** y **Administrador** comparten el Design System pero **no** comparten objetivos ni navegación. Tendrán layouts separados.
