# HU-01 · Listar pedidos con filtros

> Estado: **aprobada** por el Product Owner (Julián Gutiérrez) el 2026-11-09. Plan: [plan.md](plan.md).

## Historia de usuario

Como vendedor, quiero listar pedidos filtrando por cliente, estado y rango de fechas.

## Objetivo

Que el vendedor encuentre rápido los pedidos que le interesan sin revisarlos uno por uno: un listado paginado de pedidos que se puede acotar por cliente (código exacto o parte del nombre), por estado y por un rango de fechas, combinando los filtros que necesite y viendo primero los pedidos más recientes.

## Endpoint

`GET /api/pedidos`

| Parámetro | Tipo | Obligatorio | Descripción |
|---|---|---|---|
| `clienteId` | texto | No | Código exacto del cliente (ej.: `C-001`). |
| `cliente` | texto | No | Parte del nombre del cliente (ej.: `clavo`). |
| `estado` | texto | No | Uno de los 5 estados válidos (RN7). |
| `desde` | fecha `yyyy-MM-dd` | Solo junto con `hasta` | Inicio del rango, inclusivo. |
| `hasta` | fecha `yyyy-MM-dd` | Solo junto con `desde` | Fin del rango, inclusivo. |
| `pagina` | entero | No (defecto 1) | Número de página, desde 1. |
| `tamano` | entero | No (defecto 20) | Pedidos por página, de 1 a 100. |

## Reglas de negocio

| # | Regla |
|---|---|
| RN1 | Todos los filtros son opcionales. Sin filtros se listan todos los pedidos (paginados). |
| RN2 | Los filtros se combinan con Y: un pedido aparece solo si cumple todos los filtros enviados. |
| RN3 | Se puede filtrar por cliente de dos formas: `clienteId` (código exacto) o `cliente` (parte del nombre). |
| RN4 | El rango de fechas es inclusivo: `desde` y `hasta` incluyen ese día. |
| RN5 | `desde` y `hasta` van juntos: si se envía uno sin el otro, es un error 400. |
| RN6 | El resultado se ordena por `Fecha` descendente (más recientes primero); a igual fecha, por `Id` descendente. |
| RN7 | Estados válidos para filtrar: `Pendiente`, `Confirmado`, `Despachado`, `Entregado` y `Anulado`. Cualquier otro texto es un error 400. |
| RN8 | Si `desde` es mayor que `hasta` (fechas invertidas), es un error 400. |
| RN9 | El listado se pagina con `pagina` (defecto 1) y `tamano` (defecto 20, máximo 100). |
| RN10 | El filtro `cliente` (nombre) no distingue mayúsculas de minúsculas, pero sí tildes: `clavo` = `CLAVO`, pero `dona` no encuentra «Doña». |
| RN11 | El filtro `estado` no distingue mayúsculas de minúsculas: `entregado` vale como `Entregado`. |
| RN12 | Si llegan `clienteId` y `cliente` a la vez, se aplican los dos (Y, según RN2). |
| RN13 | Un filtro sin coincidencias (por ejemplo, un cliente que no existe) responde 200 con `pedidos` vacío y `total` 0. No es un error. |
| RN14 | La respuesta es un objeto `{ pagina, tamano, total, pedidos }`. `total` es la cantidad de pedidos que cumplen los filtros, antes de paginar. |
| RN15 | `pagina` < 1, `tamano` < 1 o `tamano` > 100 es un error 400. Una página mayor que la última responde 200 con `pedidos` vacío y el `total` real. |
| RN16 | Las fechas se reciben en formato `yyyy-MM-dd`. Una fecha con otro formato o inexistente es un error 400. |
| RN17 | Todos los errores se informan con `ProblemDetails` (RFC 9457) y código 400. |

## Criterios de aceptación

Con los 45 pedidos de ejemplo del repositorio (`PedidoRepositorioEnMemoria`). Si no se indica `pagina` ni `tamano`, se usan los valores por defecto (1 y 20).

- [ ] CA1 · Sin filtros: `total` 45, `pagina` 1, `tamano` 20 y 20 pedidos; el primero es el Id 21 (2026-10-31) y el último el Id 18.
- [ ] CA2 · `clienteId=C-001`: `total` 5, en este orden: 8, 16, 24, 32, 40.
- [ ] CA3 · `cliente=clavo`: los mismos 5 pedidos de C-001, en el mismo orden.
- [ ] CA4 · `estado=Entregado`: `total` 15.
- [ ] CA5 · `desde=2026-10-01&hasta=2026-10-31`: `total` 23 (todo octubre).
- [ ] CA6 · `desde=2026-09-02&hasta=2026-09-02`: solo el Id 40 (los extremos se incluyen).
- [ ] CA7 · `clienteId=C-001&estado=Entregado`: `total` 3, pedidos 8, 24, 40.
- [ ] CA8 · `clienteId=C-001&estado=Entregado&desde=2026-10-01&hasta=2026-10-31`: solo el Id 8.
- [ ] CA9 · `clienteId=C-999` (no existe): 200, `total` 0 y `pedidos` vacío.
- [ ] CA10 · Solo `desde=2026-10-01` (sin `hasta`), o solo `hasta`: 400 con `ProblemDetails`.
- [ ] CA11 · `estado=Cancelado`: 400 con `ProblemDetails`. `estado=entregado` (minúsculas): `total` 15.
- [ ] CA12 · `desde=2026-10-31&hasta=2026-10-01` (fechas invertidas): 400 con `ProblemDetails`.
- [ ] CA13 · `pagina=3`: `total` 45 y los pedidos 36, 17, 38, 19, 40. `pagina=4`: `total` 45 y `pedidos` vacío.
- [ ] CA14 · `pagina=0`, `tamano=0` o `tamano=101`: 400 con `ProblemDetails`.
- [ ] CA15 · `cliente=CLAVO`: los 5 pedidos de C-001. `cliente=dona`: `total` 0 (no ignora tildes).
- [ ] CA16 · `clienteId=C-001&cliente=rosa`: `total` 0 (se aplican los dos filtros).
- [ ] CA17 · `desde=2026-13-01&hasta=2026-12-31` (fecha inexistente): 400 con `ProblemDetails`.

## Ejemplos de entrada y salida

| Entrada | Salida |
|---|---|
| `GET /api/pedidos?clienteId=C-001&estado=Entregado` | 200 · `total` 3 · Ids 8, 24, 40 |
| `GET /api/pedidos?cliente=clavo&desde=2026-10-01&hasta=2026-10-31` | 200 · `total` 2 · Ids 8, 16 |
| `GET /api/pedidos?pagina=3` | 200 · `total` 45 · Ids 36, 17, 38, 19, 40 |
| `GET /api/pedidos?desde=2026-10-01` | 400 · `ProblemDetails` |

Respuesta de `GET /api/pedidos?clienteId=C-001&estado=Entregado`:

```json
{
  "pagina": 1,
  "tamano": 20,
  "total": 3,
  "pedidos": [
    {"id":8,"folio":"PED-STGO-000008","clienteId":"C-001","clienteNombre":"Ferretería El Clavo","fecha":"2026-10-20","estado":"Entregado","total":120100,"sucursal":"STGO"},
    {"id":24,"folio":"PED-STGO-000024","clienteId":"C-001","clienteNombre":"Ferretería El Clavo","fecha":"2026-09-26","estado":"Entregado","total":101300,"sucursal":"STGO"},
    {"id":40,"folio":"PED-STGO-000040","clienteId":"C-001","clienteNombre":"Ferretería El Clavo","fecha":"2026-09-02","estado":"Entregado","total":82500,"sucursal":"STGO"}
  ]
}
```

## Errores esperados

Todos responden **400** con `ProblemDetails` (RFC 9457).

| Caso | Regla |
|---|---|
| Solo `desde` o solo `hasta` | RN5 |
| Estado fuera de la lista válida | RN7 |
| `desde` mayor que `hasta` | RN8 |
| `pagina` < 1, `tamano` < 1 o `tamano` > 100 | RN15 |
| Fecha con formato distinto de `yyyy-MM-dd` o inexistente | RN16 |

Ejemplo de error (`GET /api/pedidos?desde=2026-10-31&hasta=2026-10-01`):

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Filtros inválidos",
  "status": 400,
  "detail": "La fecha 'desde' (2026-10-31) no puede ser mayor que 'hasta' (2026-10-01)."
}
```

## Fuera de alcance

- Crear, editar o anular pedidos.
- Filtrar por sucursal o por monto.
- Ignorar tildes en la búsqueda por nombre.
- Base de datos: los datos siguen en memoria hasta la Unidad 5.
- Autenticación y permisos del vendedor.

## Preguntas abiertas

Ninguna. Todas las decisiones fueron tomadas por el Product Owner en la entrevista.
