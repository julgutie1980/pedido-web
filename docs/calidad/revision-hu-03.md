# Revisión de HU-03 · Pedidos por rango de fechas

> Registro de la etapa 2 y 3 del taller U7, en lugar de un PR en GitHub. Sigue las secciones de
> `.github/pull_request_template.md`.
>
> **Adaptaciones:** la cuenta de Claude (Team) no puede vincularse a este GitHub personal, así que
> `claude-review.yml` se omite (guarda sin secreto) y la revisión de IA se hizo en local con el subagente
> `.claude/agents/revisor.md`, que lee `REVIEW.md` y `AGENTS.md` desde `main`. Sin pareja: auto-revisión.
> Merge local `--no-ff` de `hu-03-pedidos-por-fecha` a `main` y push.

## Historia o spec
`specs/HU-03-pedidos-por-fecha/spec.md` · CA-01, CA-02, CA-03 y CA-04.

## Qué cambia
- Nueva operación `FiltroPedidos.EntreFechas(pedidos, desde, hasta)` en `src/PedidosWeb.Core/Pedidos/FiltroPedidos.cs`:
  devuelve, en el orden de entrada, los pedidos cuya `Fecha` está entre `desde` y `hasta`, **ambos límites inclusive**.
- Si `desde > hasta`, lanza `ArgumentException` (parámetro `desde`): «La fecha 'desde' (yyyy-MM-dd) no puede ser mayor
  que 'hasta' (yyyy-MM-dd).». Si `pedidos` es nulo, `ArgumentNullException`.
- El formato de fecha del mensaje reutiliza `ListadoPedidos.FormatoFecha`.
- 6 tests en `tests/PedidosWeb.Core.Tests/Pedidos/FiltroPedidosTests.cs`: uno por CA (4), el de pedidos nulo y el de
  rango de un solo día.

## Por qué
El encargado de ventas necesita ver lo vendido en un período exacto (un mes, una semana, un día). Los límites son
inclusive para no perder pedidos del primer ni del último día, y un rango invertido falla con un mensaje claro en vez
de devolver una lista vacía que parezca «no hubo ventas». Es un cambio chico en Core, sin API, para que sea fácil de
revisar.

## Cómo probarlo
1. `git switch main && git pull` (o la rama `hu-03-pedidos-por-fecha`).
2. `dotnet test --filter FullyQualifiedName~FiltroPedidosTests` → 6 tests en verde.
3. `dotnet test` → 126 tests en verde.

## Fuera de alcance
- Exponer la ruta en la API.
- Cambiar `IPedidoRepositorio`, los pedidos de ejemplo o `ListadoPedidos` (HU-01).

## Hallazgos del revisor (IA, local) y respuesta del autor
Primera revisión (commit `ad22600`): 0 Importantes, 3 Detalles, veredicto «Listo para PR».

| # | archivo:línea | Hallazgo de la IA | Respuesta |
|---|---|---|---|
| 1 | `FiltroPedidos.cs:21` | `ThrowIfNull(pedidos)` sin test: si se borra, ningún test falla. | **Acepto:** agregué `EntreFechas_PedidosNulo_LanzaArgumentNullException`. REVIEW.md › Revisar siempre 2 pide cubrir nulos; un guard sin test puede desaparecer sin aviso. |
| 2 | `FiltroPedidos.cs:11` | `FormatoFecha` duplica la constante pública `ListadoPedidos.FormatoFecha` (`ListadoPedidos.cs:13`). | **Acepto:** uso `ListadoPedidos.FormatoFecha` y borré la constante local. Una sola fuente para el formato; cambio de 2 líneas. |
| 3 | `FiltroPedidos.cs:29` | La regla de rango inclusivo está duplicada en `ListadoPedidos.cs:69`. | **Descarto en este PR:** la spec deja fuera de alcance otras historias (`spec.md:25-28`) y la guía pide PR chico. Unificar `ListadoPedidos` (HU-01) va en un PR propio, con sus tests. |

Lo acordado se aplicó en el commit `1d455ed` (`fix: aplica lo acordado en la revisión de HU-03`) y el revisor se volvió a
correr sobre la rama.

## Hallazgo humano
**Rango de un solo día sin test.** `desde == hasta` es válido (no lanza) y debe devolver los pedidos de ese día, pero
ningún test lo fijaba: cambiar `>` por `>=` en la validación (`FiltroPedidos.cs`, `if (desde > hasta)`) habría
rechazado ese caso sin que fallara nada. La IA no lo vio porque los cuatro CA pasaban. Se agregó
`EntreFechas_DesdeIgualAHasta_DevuelveLosPedidosDeEseDia`.

## Antes de pedir revisión
- [x] `dotnet test` en verde y **un test por criterio de aceptación** (`Metodo_Escenario_ResultadoEsperado`).
- [x] **Corrí el subagente revisor en local** y respondí cada hallazgo arriba.
