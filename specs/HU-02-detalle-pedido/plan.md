# Plan · HU-02 Detalle de un pedido

> Plan de implementación de [spec.md](spec.md). Se construye en 3 incrementos, cada uno con sus tests y su commit.

## Qué se reutiliza

- `Pedido` e `IPedidoRepositorio.ObtenerPorId(int)` (`src/PedidosWeb.Core/Pedidos/`).
- `LineaPedido` e `ILineaPedidoRepositorio.ObtenerPorPedido(int)`, que ya viene ordenado por número (`src/PedidosWeb.Core/Lineas/`).
- El nombre del cliente sale de `Pedido.ClienteNombre`. No se usa `ClientesService`, que se ordena en la etapa 2.
- Comprobación del pedido 8: 3×21.500 + 12×2.950 + 10×2.020 = **120.100**.

## Incremento 1 de 3 · Calcular el total

- **Toca solo:** archivos nuevos en `src/PedidosWeb.Core/Pedidos/` y `tests/PedidosWeb.Core.Tests/Pedidos/`.
- `CalculadoraTotalPedido` (estática):
  - `Subtotal(LineaPedido)` devuelve `Cantidad × PrecioUnitario`.
  - `CalcularTotal(IEnumerable<LineaPedido>)` devuelve la suma de los subtotales (0 si no hay líneas).
- Test `CalculadoraTotalPedidoTests`, `// CA-01`: las líneas del pedido 8 suman 120100.
- **Commit:** `feat: HU-02 incremento 1, total del pedido desde sus líneas`.

## Incremento 2 de 3 · Obtener el detalle

- **Toca solo:** archivos nuevos en `Pedidos/` y sus tests. No modifica la API.
- Records:
  - `LineaDetalle(Numero, Producto, Cantidad, PrecioUnitario, Subtotal)`.
  - `DetallePedido(Id, Folio, Fecha, Estado, Sucursal, ClienteId, ClienteNombre, Lineas, Total)`.
- `DetallePedidoServicio(IPedidoRepositorio, ILineaPedidoRepositorio)`:
  - `ObtenerDetalle(int id)` devuelve `DetallePedido?`.
  - Si el pedido no existe, devuelve `null`.
- Tests `DetallePedidoServicioTests`:
  - `// CA-01`: el total del detalle es 120100.
  - `// CA-02`: folio, estado, cliente y las 3 líneas ordenadas.
  - `// CA-03`: el id 999 da `null`.
- **Commit:** `feat: HU-02 incremento 2, detalle del pedido en Core`.

## Incremento 3 de 3 · Publicarlo en la API

- **Toca solo:** `src/PedidosWeb.Api/Program.cs`. No modifica `GET /api/pedidos/{id}` ni `GET /api/pedidos`.
- Registra `ILineaPedidoRepositorio` → `LineaPedidoRepositorioEnMemoria` y `DetallePedidoServicio` como singletons.
- `GET /api/pedidos/{id:int}/detalle` devuelve `200` con el detalle o `404` (CA-03).
- **Verificación en el navegador:**
  - `/api/pedidos/8/detalle` muestra un total de 120100.
  - `/api/pedidos/999/detalle` responde 404.
- **Commit:** `feat: HU-02 incremento 3, ruta GET /api/pedidos/{id}/detalle`.

## Verificación de cada incremento

- `dotnet build` y `dotnet test` en verde.
- El diff se lee en 5 minutos.
- Commit y push solo con autorización de Julián.
