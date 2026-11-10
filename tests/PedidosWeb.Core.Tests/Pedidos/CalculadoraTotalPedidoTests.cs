using PedidosWeb.Core.Lineas;
using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Pedidos;

/// <summary>
/// Incremento 1 de specs/HU-02-detalle-pedido/spec.md: el total se calcula desde las líneas.
/// </summary>
public class CalculadoraTotalPedidoTests
{
    // CA-01
    [Fact]
    public void CalcularTotal_LineasDelPedido8_Devuelve120100()
    {
        var lineas = new LineaPedidoRepositorioEnMemoria().ObtenerPorPedido(8);

        var total = CalculadoraTotalPedido.CalcularTotal(lineas);

        Assert.Equal(3, lineas.Count);
        Assert.Equal(120100m, total);
        Assert.Equal(lineas.Sum(CalculadoraTotalPedido.Subtotal), total);
    }
}
