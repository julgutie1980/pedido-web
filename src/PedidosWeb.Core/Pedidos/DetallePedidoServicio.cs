using PedidosWeb.Core.Lineas;

namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Arma el detalle de un pedido (HU-02) con sus líneas y el total calculado desde ellas.
/// </summary>
public sealed class DetallePedidoServicio
{
    private readonly IPedidoRepositorio _pedidos;
    private readonly ILineaPedidoRepositorio _lineas;

    public DetallePedidoServicio(IPedidoRepositorio pedidos, ILineaPedidoRepositorio lineas)
    {
        _pedidos = pedidos;
        _lineas = lineas;
    }

    /// <summary>El detalle del pedido con ese id, o null si el pedido no existe.</summary>
    public DetallePedido? ObtenerDetalle(int id)
    {
        if (_pedidos.ObtenerPorId(id) is not { } pedido)
        {
            return null;
        }

        var lineas = _lineas.ObtenerPorPedido(id);

        var lineasDetalle = lineas
            .Select(l => new LineaDetalle(
                l.Numero, l.Producto, l.Cantidad, l.PrecioUnitario, CalculadoraTotalPedido.Subtotal(l)))
            .ToList();

        return new DetallePedido(
            pedido.Id,
            pedido.Folio,
            pedido.Fecha,
            pedido.Estado,
            pedido.Sucursal,
            pedido.ClienteId,
            pedido.ClienteNombre,
            lineasDetalle,
            CalculadoraTotalPedido.CalcularTotal(lineas));
    }
}
