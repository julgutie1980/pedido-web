using PedidosWeb.Core.Pedidos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPedidoRepositorio, PedidoRepositorioEnMemoria>();
builder.Services.AddSingleton<ListadoPedidos>();

var app = builder.Build();

// Para comprobar que la API está arriba.
app.MapGet("/salud", () => Results.Ok(new { estado = "ok" }));

// Detalle de un pedido por id.
app.MapGet("/api/pedidos/{id:int}", (int id, IPedidoRepositorio repositorio) =>
    repositorio.ObtenerPorId(id) is { } pedido
        ? Results.Ok(pedido)
        : Results.NotFound());

// HU-01 · Listar pedidos con filtros (specs/HU-01-listar-pedidos/spec.md).
// La lógica vive en Core (ListadoPedidos); aquí solo se traduce el resultado a HTTP.
app.MapGet("/api/pedidos", (
    string? clienteId,
    string? cliente,
    string? estado,
    string? desde,
    string? hasta,
    int? pagina,
    int? tamano,
    ListadoPedidos listado) =>
{
    var resultado = listado.Listar(new ConsultaPedidos(clienteId, cliente, estado, desde, hasta, pagina, tamano));

    return resultado.EsValido
        ? Results.Ok(resultado.Pagina)
        : Results.Problem(statusCode: StatusCodes.Status400BadRequest, title: "Filtros inválidos", detail: resultado.Error);
});

app.Run();

// Permite crear tests de integración con WebApplicationFactory<Program> más adelante.
public partial class Program { }
