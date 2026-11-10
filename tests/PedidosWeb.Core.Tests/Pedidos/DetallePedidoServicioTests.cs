using PedidosWeb.Core.Lineas;
using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Pedidos;

/// <summary>
/// Incremento 2 de specs/HU-02-detalle-pedido/spec.md: el detalle del pedido se arma en Core.
/// </summary>
public class DetallePedidoServicioTests
{
    private readonly DetallePedidoServicio _servicio =
        new(new PedidoRepositorioEnMemoria(), new LineaPedidoRepositorioEnMemoria());

    // CA-01
    [Fact]
    public void ObtenerDetalle_Pedido8_TotalEsLaSumaDeLosSubtotales()
    {
        var detalle = _servicio.ObtenerDetalle(8);

        Assert.NotNull(detalle);
        Assert.Equal(120100m, detalle.Total);
        Assert.Equal(detalle.Lineas.Sum(l => l.Subtotal), detalle.Total);
    }

    // CA-02
    [Fact]
    public void ObtenerDetalle_Pedido8_TraeFolioEstadoClienteYLineasOrdenadas()
    {
        var detalle = _servicio.ObtenerDetalle(8);

        Assert.NotNull(detalle);
        Assert.Equal(8, detalle.Id);
        Assert.Equal("PED-STGO-000008", detalle.Folio);
        Assert.Equal("Entregado", detalle.Estado);
        Assert.Equal("C-001", detalle.ClienteId);
        Assert.Equal("Ferretería El Clavo", detalle.ClienteNombre);
        Assert.Equal(new[] { 1, 2, 3 }, detalle.Lineas.Select(l => l.Numero));
    }

    // CA-03
    [Fact]
    public void ObtenerDetalle_PedidoInexistente_DevuelveNull()
    {
        var detalle = _servicio.ObtenerDetalle(999);

        Assert.Null(detalle);
    }
}
