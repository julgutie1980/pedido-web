using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Pedidos;

/// <summary>
/// Un test por criterio de aceptación de specs/HU-01-listar-pedidos/spec.md,
/// con los 45 pedidos de ejemplo de PedidoRepositorioEnMemoria.
/// </summary>
public class ListadoPedidosTests
{
    private readonly ListadoPedidos _listado = new(new PedidoRepositorioEnMemoria());

    private static ConsultaPedidos Consulta(
        string? clienteId = null,
        string? cliente = null,
        string? estado = null,
        string? desde = null,
        string? hasta = null,
        int? pagina = null,
        int? tamano = null) =>
        new(clienteId, cliente, estado, desde, hasta, pagina, tamano);

    private static int[] Ids(ResultadoListado resultado) =>
        resultado.Pagina!.Pedidos.Select(p => p.Id).ToArray();

    private static void AssertValido(ResultadoListado resultado)
    {
        Assert.True(resultado.EsValido, resultado.Error);
        Assert.NotNull(resultado.Pagina);
    }

    private static void AssertError(ResultadoListado resultado)
    {
        Assert.False(resultado.EsValido);
        Assert.False(string.IsNullOrWhiteSpace(resultado.Error));
        Assert.Null(resultado.Pagina);
    }

    // CA-01
    [Fact]
    public void Listar_SinFiltros_DevuelvePrimeraPaginaDe20ConTotal45()
    {
        var resultado = _listado.Listar(Consulta());

        AssertValido(resultado);
        Assert.Equal(45, resultado.Pagina!.Total);
        Assert.Equal(1, resultado.Pagina.Pagina);
        Assert.Equal(20, resultado.Pagina.Tamano);
        Assert.Equal(20, resultado.Pagina.Pedidos.Count);
        Assert.Equal(21, resultado.Pagina.Pedidos[0].Id);
        Assert.Equal(18, resultado.Pagina.Pedidos[^1].Id);
    }

    // CA-02
    [Fact]
    public void Listar_PorClienteId_DevuelveLos5PedidosDeC001Ordenados()
    {
        var resultado = _listado.Listar(Consulta(clienteId: "C-001"));

        AssertValido(resultado);
        Assert.Equal(5, resultado.Pagina!.Total);
        Assert.Equal(new[] { 8, 16, 24, 32, 40 }, Ids(resultado));
    }

    // CA-03
    [Fact]
    public void Listar_PorParteDelNombre_DevuelveLosPedidosDeFerreteriaElClavo()
    {
        var resultado = _listado.Listar(Consulta(cliente: "clavo"));

        AssertValido(resultado);
        Assert.Equal(new[] { 8, 16, 24, 32, 40 }, Ids(resultado));
    }

    // CA-04
    [Fact]
    public void Listar_PorEstadoEntregado_DevuelveTotal15()
    {
        var resultado = _listado.Listar(Consulta(estado: "Entregado"));

        AssertValido(resultado);
        Assert.Equal(15, resultado.Pagina!.Total);
        Assert.All(resultado.Pagina.Pedidos, p => Assert.Equal("Entregado", p.Estado));
    }

    // CA-05
    [Fact]
    public void Listar_RangoOctubre_DevuelveTotal23()
    {
        var resultado = _listado.Listar(Consulta(desde: "2026-10-01", hasta: "2026-10-31"));

        AssertValido(resultado);
        Assert.Equal(23, resultado.Pagina!.Total);
    }

    // CA-06
    [Fact]
    public void Listar_RangoDeUnDia_IncluyeLosExtremos()
    {
        var resultado = _listado.Listar(Consulta(desde: "2026-09-02", hasta: "2026-09-02"));

        AssertValido(resultado);
        Assert.Equal(new[] { 40 }, Ids(resultado));
    }

    // CA-07
    [Fact]
    public void Listar_ClienteYEstado_CombinaLosFiltros()
    {
        var resultado = _listado.Listar(Consulta(clienteId: "C-001", estado: "Entregado"));

        AssertValido(resultado);
        Assert.Equal(3, resultado.Pagina!.Total);
        Assert.Equal(new[] { 8, 24, 40 }, Ids(resultado));
    }

    // CA-08
    [Fact]
    public void Listar_ClienteEstadoYRango_DevuelveSoloElPedido8()
    {
        var resultado = _listado.Listar(Consulta(
            clienteId: "C-001", estado: "Entregado", desde: "2026-10-01", hasta: "2026-10-31"));

        AssertValido(resultado);
        Assert.Equal(new[] { 8 }, Ids(resultado));
    }

    // CA-09
    [Fact]
    public void Listar_ClienteInexistente_DevuelveListaVaciaSinError()
    {
        var resultado = _listado.Listar(Consulta(clienteId: "C-999"));

        AssertValido(resultado);
        Assert.Equal(0, resultado.Pagina!.Total);
        Assert.Empty(resultado.Pagina.Pedidos);
    }

    // CA-10
    [Theory]
    [InlineData("2026-10-01", null)]
    [InlineData(null, "2026-10-31")]
    public void Listar_SoloDesdeOSoloHasta_DevuelveError(string? desde, string? hasta)
    {
        var resultado = _listado.Listar(Consulta(desde: desde, hasta: hasta));

        AssertError(resultado);
    }

    // CA-11
    [Theory]
    [InlineData("Cancelado", false, 0)]
    [InlineData("entregado", true, 15)]
    public void Listar_Estado_ValidaListaEIgnoraMayusculas(string estado, bool esValido, int totalEsperado)
    {
        var resultado = _listado.Listar(Consulta(estado: estado));

        if (esValido)
        {
            AssertValido(resultado);
            Assert.Equal(totalEsperado, resultado.Pagina!.Total);
        }
        else
        {
            AssertError(resultado);
        }
    }

    // CA-12
    [Fact]
    public void Listar_FechasInvertidas_DevuelveError()
    {
        var resultado = _listado.Listar(Consulta(desde: "2026-10-31", hasta: "2026-10-01"));

        AssertError(resultado);
    }

    // CA-13
    [Theory]
    [InlineData(3, new[] { 36, 17, 38, 19, 40 })]
    [InlineData(4, new int[0])]
    public void Listar_Paginacion_DevuelvePaginaPedidaOVacia(int pagina, int[] idsEsperados)
    {
        var resultado = _listado.Listar(Consulta(pagina: pagina));

        AssertValido(resultado);
        Assert.Equal(45, resultado.Pagina!.Total);
        Assert.Equal(pagina, resultado.Pagina.Pagina);
        Assert.Equal(idsEsperados, Ids(resultado));
    }

    // CA-14
    [Theory]
    [InlineData(0, null)]
    [InlineData(null, 0)]
    [InlineData(null, 101)]
    public void Listar_PaginaOTamanoFueraDeRango_DevuelveError(int? pagina, int? tamano)
    {
        var resultado = _listado.Listar(Consulta(pagina: pagina, tamano: tamano));

        AssertError(resultado);
    }

    // CA-15
    [Theory]
    [InlineData("CLAVO", 5)]
    [InlineData("dona", 0)]
    public void Listar_Nombre_IgnoraMayusculasPeroNoTildes(string cliente, int totalEsperado)
    {
        var resultado = _listado.Listar(Consulta(cliente: cliente));

        AssertValido(resultado);
        Assert.Equal(totalEsperado, resultado.Pagina!.Total);
    }

    // CA-16
    [Fact]
    public void Listar_ClienteIdYNombreContradictorios_DevuelveTotal0()
    {
        var resultado = _listado.Listar(Consulta(clienteId: "C-001", cliente: "rosa"));

        AssertValido(resultado);
        Assert.Equal(0, resultado.Pagina!.Total);
    }

    // CA-17
    [Fact]
    public void Listar_FechaInexistente_DevuelveError()
    {
        var resultado = _listado.Listar(Consulta(desde: "2026-13-01", hasta: "2026-12-31"));

        AssertError(resultado);
    }
}
