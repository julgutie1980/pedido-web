# Plan de implementación · HU-01 Listar pedidos con filtros

> Estado: **aprobado** por el Product Owner (Julián Gutiérrez) el 2026-11-09. Spec: [spec.md](spec.md).

## Contexto
La spec `specs/HU-01-listar-pedidos/spec.md` ya no tiene decisiones pendientes (17 reglas, 17 criterios). AGENTS.md exige: spec aprobada antes de codificar, un test xUnit por criterio de aceptación y no hacer commit sin tests en verde. Julián decidió probar todo en `PedidosWeb.Core.Tests`, **sin paquetes NuGet nuevos ni proyectos nuevos**: la lógica vive en Core y la API solo traduce el resultado a HTTP.

## Enfoque
- **Core** hace todo el trabajo: valida la consulta, filtra, ordena y pagina. Recibe los parámetros como texto (fechas) y enteros opcionales, así la validación de formato (RN16) también queda en Core y se puede testear.
- **Api** solo mapea: consulta válida → `200` con el objeto de página; consulta inválida → `Results.Problem(statusCode: 400, title: "Filtros inválidos", detail: <mensaje>)`.
- Se reutiliza `IPedidoRepositorio.ObtenerTodos()` (`src/PedidosWeb.Core/Pedidos/IPedidoRepositorio.cs`); no se cambia el repositorio ni el modelo `Pedido`.
- Comparaciones con `StringComparison.OrdinalIgnoreCase`: ignora mayúsculas pero **no** tildes (RN10, RN11). `clienteId` con `StringComparison.Ordinal` (exacto).
- Fechas con `DateOnly.TryParseExact(valor, "yyyy-MM-dd", CultureInfo.InvariantCulture, ...)`.
- Validación en el orden: fechas (formato → pareja → invertidas), estado, pagina/tamano. Se devuelve el primer error.

## Archivos
| Archivo | Cambio |
|---|---|
| `src/PedidosWeb.Core/Pedidos/ConsultaPedidos.cs` (nuevo) | `record ConsultaPedidos(string? ClienteId, string? Cliente, string? Estado, string? Desde, string? Hasta, int? Pagina, int? Tamano)` |
| `src/PedidosWeb.Core/Pedidos/PaginaPedidos.cs` (nuevo) | `record PaginaPedidos(int Pagina, int Tamano, int Total, IReadOnlyList<Pedido> Pedidos)` |
| `src/PedidosWeb.Core/Pedidos/ResultadoListado.cs` (nuevo) | Resultado: `PaginaPedidos? Pagina` o `string? Error` (+ `EsValido`) |
| `src/PedidosWeb.Core/Pedidos/ListadoPedidos.cs` (nuevo) | Clase que recibe `IPedidoRepositorio`; método `Listar(ConsultaPedidos)`; constantes: estados válidos, `TamanoPorDefecto = 20`, `TamanoMaximo = 100` |
| `src/PedidosWeb.Api/Program.cs` | Registrar `ListadoPedidos` (singleton) y agregar `GET /api/pedidos` con los 7 parámetros de query; quitar el comentario «PENDIENTE» |
| `tests/PedidosWeb.Core.Tests/Pedidos/ListadoPedidosTests.cs` (nuevo) | 17 tests, uno por criterio, con `PedidoRepositorioEnMemoria` real |
| `specs/HU-01-listar-pedidos/plan.md` (nuevo) | Copia de este plan (convención de `specs/README.md`) |

## Tests (uno por criterio) · `ListadoPedidosTests`
| CA | Test |
|---|---|
| 1 | `Listar_SinFiltros_DevuelvePrimeraPaginaDe20ConTotal45` (primero Id 21, último Id 18) |
| 2 | `Listar_PorClienteId_DevuelveLos5PedidosDeC001Ordenados` (8,16,24,32,40) |
| 3 | `Listar_PorParteDelNombre_DevuelveLosPedidosDeFerreteriaElClavo` |
| 4 | `Listar_PorEstadoEntregado_DevuelveTotal15` |
| 5 | `Listar_RangoOctubre_DevuelveTotal23` |
| 6 | `Listar_RangoDeUnDia_IncluyeLosExtremos` (Id 40) |
| 7 | `Listar_ClienteYEstado_CombinaLosFiltros` (8,24,40) |
| 8 | `Listar_ClienteEstadoYRango_DevuelveSoloElPedido8` |
| 9 | `Listar_ClienteInexistente_DevuelveListaVaciaSinError` |
| 10 | `Listar_SoloDesdeOSoloHasta_DevuelveError` (Theory, 2 casos) |
| 11 | `Listar_Estado_ValidaListaEIgnoraMayusculas` (Theory: `Cancelado` → error; `entregado` → 15) |
| 12 | `Listar_FechasInvertidas_DevuelveError` |
| 13 | `Listar_Paginacion_DevuelvePaginaPedidaOVacia` (Theory: pág. 3 → 36,17,38,19,40; pág. 4 → vacía, total 45) |
| 14 | `Listar_PaginaOTamanoFueraDeRango_DevuelveError` (Theory: pagina 0, tamano 0, tamano 101) |
| 15 | `Listar_Nombre_IgnoraMayusculasPeroNoTildes` (Theory: `CLAVO` → 5; `dona` → 0) |
| 16 | `Listar_ClienteIdYNombreContradictorios_DevuelveTotal0` |
| 17 | `Listar_FechaInexistente_DevuelveError` (`2026-13-01`) |

## Riesgos
- **El 400 con ProblemDetails no tiene test automático** (decisión de no agregar NuGet): se verifica a mano con la API levantada. Queda pendiente para cuando se agreguen tests de integración.
- **Parámetros no numéricos** (`pagina=abc`): la Minimal API responde 400 antes de llegar a Core, sin ProblemDetails. La spec no lo cubre; se puede agregar `builder.Services.AddProblemDetails()` para uniformarlo (no requiere NuGet).
- **Tildes**: usar `OrdinalIgnoreCase` y no `CurrentCultureIgnoreCase` ni normalización, o `dona` encontraría «Doña» y CA15 fallaría.
- **Datos fijos**: los tests dependen de los 45 pedidos de ejemplo; si cambian los datos (U5, PostgreSQL), hay que revisar los números.
- **Commit pendiente**: la base de la U2 aún no tiene commit; conviene hacerlo antes, para que el diff de HU-01 quede separado.

## Pasos
0. (Julián o con su autorización) Commit de la base U2 en `unidad-02`: `chore: base de la Unidad 2 (API y pedidos de ejemplo)`.
1. Marcar la spec como **aprobada** y guardar este plan en `specs/HU-01-listar-pedidos/plan.md`.
2. Escribir los 17 tests en `ListadoPedidosTests.cs` → `dotnet test` en rojo (no compila o falla: esperado).
3. Crear `ConsultaPedidos`, `PaginaPedidos`, `ResultadoListado` y `ListadoPedidos` en Core → `dotnet test` en verde (19 + 17 casos).
4. Agregar `GET /api/pedidos` en `Program.cs`.
5. Verificación manual de la API y entrega del diff para revisión (sin commit hasta que Julián lo autorice).

## Verificación
- `dotnet build`: 0 errores, 0 advertencias. `dotnet test`: todo en verde.
- Con `dotnet run --project src/PedidosWeb.Api`:
  - `/api/pedidos?clienteId=C-001&estado=Entregado` → 200, `total` 3, Ids 8, 24, 40.
  - `/api/pedidos?desde=2026-10-31&hasta=2026-10-01` → 400 con `ProblemDetails`.
  - `/api/pedidos?pagina=4` → 200, `pedidos` vacío, `total` 45.
- Cada criterio de la spec tiene su test con el número de CA en un comentario.
