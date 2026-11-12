using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Pedidos;

/// <summary>
/// Un test por criterio de aceptación de specs/HU-03-pedidos-por-fecha/spec.md.
/// </summary>
public class FiltroPedidosTests
{
    private static readonly DateOnly Desde = new(2026, 3, 1);
    private static readonly DateOnly Hasta = new(2026, 3, 31);

    private static Pedido PedidoDel(int id, DateOnly fecha) =>
        new(id, $"PED-STGO-{id:000000}", "C-001", "Cliente de prueba", fecha, "Confirmado", 10000m, "STGO");

    // CA-01
    [Fact]
    public void EntreFechas_FechaIgualADesde_IncluyeElPedido()
    {
        var pedido = PedidoDel(1, Desde);

        var resultado = FiltroPedidos.EntreFechas([pedido], Desde, Hasta);

        Assert.Equal([pedido], resultado);
    }

    // CA-02
    [Fact]
    public void EntreFechas_FechaIgualAHasta_IncluyeElPedido()
    {
        var pedido = PedidoDel(2, Hasta);

        var resultado = FiltroPedidos.EntreFechas([pedido], Desde, Hasta);

        Assert.Equal([pedido], resultado);
    }

    // CA-03
    [Fact]
    public void EntreFechas_FechaFueraDeRango_ExcluyeElPedido()
    {
        var antes = PedidoDel(3, Desde.AddDays(-1));
        var dentro = PedidoDel(4, new DateOnly(2026, 3, 15));
        var despues = PedidoDel(5, Hasta.AddDays(1));

        var resultado = FiltroPedidos.EntreFechas([antes, dentro, despues], Desde, Hasta);

        Assert.Equal([dentro], resultado);
    }

    // CA-04
    [Fact]
    public void EntreFechas_DesdeMayorQueHasta_LanzaArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(
            () => FiltroPedidos.EntreFechas([PedidoDel(6, Desde)], Hasta, Desde));

        Assert.Equal("desde", ex.ParamName);
        Assert.Contains("'desde' (2026-03-31)", ex.Message);
        Assert.Contains("'hasta' (2026-03-01)", ex.Message);
    }

    // Revisión · hallazgo IA 1 (aceptado): el guard de nulos queda cubierto.
    [Fact]
    public void EntreFechas_PedidosNulo_LanzaArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => FiltroPedidos.EntreFechas(null!, Desde, Hasta));

        Assert.Equal("pedidos", ex.ParamName);
    }

    // Revisión · hallazgo humano: un rango de un solo día es válido y devuelve los pedidos de ese día.
    [Fact]
    public void EntreFechas_DesdeIgualAHasta_DevuelveLosPedidosDeEseDia()
    {
        var delDia = PedidoDel(7, Desde);
        var otroDia = PedidoDel(8, Desde.AddDays(1));

        var resultado = FiltroPedidos.EntreFechas([delDia, otroDia], Desde, Desde);

        Assert.Equal([delDia], resultado);
    }
}
