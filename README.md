# Wallet

**Demo online:** [Ver Wallet](https://walletsquad1.runasp.net/)

**Desarrolladora:** Naiquen Iturralde — [Portfolio](https://naiqueniturralde.github.io/Portfolio/)

# Wallet

Wallet es una demo de billetera digital (proyecto académico/profesional de equipo) que integra una aplicación web desarrollada en Blazor con una arquitectura de datos en Google BigQuery y dashboards analíticos desarrollados en Looker Studio.

El equipo creó el dataset en BigQuery y lo cargó con datos ficticios de prueba para representar cuentas, movimientos, inversiones y auditoría. El proyecto busca integrar experiencia de usuario, navegación de una billetera digital, procesamiento y organización de datos, consultas analíticas y visualización de información.

## Tecnologías

- .NET 10 y Blazor (renderizado interactivo en servidor)
- C# y Razor Components
- HTML y CSS (CSS scoped por componente)
- JavaScript (módulo mínimo `wallet-nav.js` para el historial del navegador)
- Google BigQuery (cliente `Google.Cloud.BigQuery.V2`)
- SQL (consultas parametrizadas de solo lectura)
- Looker Studio (reportes embebidos)
- Diseño responsive mobile-first
- Git / GitHub

## Arquitectura general

```text
BigQuery (dataset del equipo con datos ficticios de prueba)
→ vistas y consultas SQL (wallet_mart / wallet_core)
→ Looker Studio (capa de visualización analítica)
→ dashboards analíticos
→ integración dentro de Wallet (reportes embebidos)
→ interfaz para Usuario y Administrador
```

La aplicación Blazor consulta BigQuery directamente desde el servidor: no existe una API ni un backend intermedio propio. Las pantallas dependen de una capa de acceso a datos única (`WalletBigQueryDataSource`) que ejecuta consultas de solo lectura y devuelve DTOs; no calcula saldos ni convierte monedas.

## Funcionalidades principales

- Dashboard de Usuario (`/wallet/dashboard`): saldo disponible, resumen del período (ingresos, egresos, balance), cuentas, movimientos recientes y reportes de Looker Studio.
- Dashboard de Administrador (`/admin/dashboard`): indicadores globales (usuarios, cuentas, transacciones, volumen aprobado por moneda, inversiones, auditorías, alertas) y reportes de Looker Studio.
- Cuentas (`/wallet/cuentas`): detalle de cada cuenta con saldo, alias, CBU y movimientos, con ARS y USD informados por separado.
- Inversiones (`/wallet/inversiones`): listado de inversiones registradas del usuario.
- Transferencias (`/wallet/transferir`): flujo visual paso a paso en preparación; todavía no ejecuta operaciones.
- Filtro por moneda (ARS | USD) en el dashboard de usuario: filtra los datos sin convertir valores.
- Navegación responsive: sidebar en escritorio y barra inferior en móvil, con navegación unificada para ambos roles.
- Integración de reportes de Looker Studio: 3 reportes para Usuario y 6 para Administrador, embebidos en cards modulares.
- Landing pública con presentación del proyecto y exhibición de los dashboards de la demo.

No están habilitados en esta etapa: perfil, historial completo de movimientos, destinatarios guardados y cambio de moneda (se muestran como secciones pendientes, sin enlaces falsos).

## Integración con datos

La solución utiliza datos provenientes del modelo desarrollado en BigQuery y Looker Studio funciona como capa de visualización. La separación es explícita:

- Datos almacenados y procesados en BigQuery: datasets `wallet_core` (dimensiones y hechos) y `wallet_mart` (vistas de reporting).
- Consultas y vistas analíticas: vistas SQL que concentran la lógica de reporteo.
- Visualización en Looker Studio: reportes oficiales embebidos en la interfaz.
- Experiencia de usuario: componentes Blazor que presentan los datos sin duplicar cálculos.

La interfaz no es la fuente de verdad: no hay datos hardcodeados ni fallback hacia datos demo ante un fallo de BigQuery; los estados de error se muestran al usuario.

## Dashboards

El sistema contiene dos perspectivas principales.

### Dashboard Usuario

Visualiza la información consultada en BigQuery (datos de prueba del equipo):

- Saldo disponible por cuenta, filtrado por moneda (ARS/USD).
- Resumen del período: ingresos, egresos y balance, también por moneda.
- Cantidad de cuentas y cuentas activas.
- Movimientos recientes con estado, tipo, fecha e importe.
- Reportes Looker Studio: ingresos y egresos por período, evolución de la actividad y distribución de egresos.

### Dashboard Administrador

Permite analizar la plataforma a nivel global:

- Usuarios totales y activos.
- Cuentas totales y activas.
- Transacciones por estado (aprobadas, pendientes, rechazadas).
- Volumen aprobado, informado por separado en ARS y USD.
- Inversiones totales y activas.
- Auditorías y errores de auditoría.
- Alertas totales y sin leer.
- Reportes Looker Studio: volumen transaccionado, altas de usuarios, transacciones por tipo, errores de auditoría por tipo, evolución de errores y estado de las transacciones.

## Informe de vistas del dashboard

Vistas verificadas en el código de acceso a datos (`WalletBigQueryDataSource.cs`).

### `wallet_mart.vw_dashboard_usuario`

**Objetivo:** concentra el resumen del período de un usuario: saldos actuales, ingresos, egresos, balance y cantidad de cuentas y movimientos, con columnas separadas por moneda (`*_ars` / `*_usd`).

**Uso en el dashboard:** alimenta el saldo disponible y la card de métricas del período del Dashboard Usuario.

**Justificación:** resuelve en SQL la agregación por usuario y período, mantiene la consistencia de las métricas y evita que la interfaz duplique lógica de cálculo o convierta monedas.

### `wallet_mart.vw_saldo_cuentas`

**Objetivo:** entrega las cuentas de un usuario con su saldo actual, créditos, débitos, alias, CBU, estado y moneda.

**Uso en el dashboard:** alimenta el bloque de saldo, la lista de cuentas, la página de Cuentas y la selección de cuenta de origen en Transferir; también resuelve la pertenencia de cuentas para inversiones.

**Justificación:** centraliza la lectura de cuentas en un solo lugar y permite filtrar por moneda dentro de la consulta, sin sumar saldos de monedas distintas en la interfaz.

### `wallet_mart.vw_transacciones_reporting`

**Objetivo:** reúne las transacciones con fecha, importe, estado, tipo y usuarios origen/destino, en formato listo para reporteo.

**Uso en el dashboard:** alimenta los movimientos recientes del Usuario y, en el Administrador, el volumen aprobado por moneda y los agregados por estado y tipo de transacción.

**Justificación:** reutiliza la lógica SQL de reporting para ambas perspectivas y desacopla el modelo transaccional de la presentación.

### `wallet_mart.vw_inversiones_reporting`

**Objetivo:** concentra las inversiones con tipo, monto invertido, tasa, fechas, plazo, interés ganado, monto resultante y estado.

**Uso en el dashboard:** alimenta la página de Inversiones del Usuario y el KPI de inversiones del Administrador.

**Justificación:** expone las inversiones en formato analítico sin exponer tablas base, manteniendo el cálculo financiero en el modelo.

### `wallet_core.vw_cuentas_activas`

**Objetivo:** filtra las cuentas en estado activo dentro del dataset operativo.

**Uso en el dashboard:** alimenta el KPI de cuentas activas del Dashboard Administrador.

**Justificación:** encapsula el criterio de "activa" en el modelo, de modo que el mismo concepto se use en todas las métricas.

## Ejecución

```bash
dotnet restore
dotnet run
```

Requisitos: SDK .NET 10 y credenciales de Google Cloud para BigQuery (Application Default Credentials, por ejemplo mediante `gcloud auth application-default login` o la variable de entorno `GOOGLE_APPLICATION_CREDENTIALS`). La configuración del proyecto y los datasets vive en `appsettings.json`, sección `WalletBigQuery`.
