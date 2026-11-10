namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Filtros y paginación del listado de pedidos (HU-01), tal como llegan del cliente.
/// Las fechas vienen como texto para validar su formato (yyyy-MM-dd) en Core.
/// </summary>
/// <param name="ClienteId">Código exacto del cliente, por ejemplo "C-001".</param>
/// <param name="Cliente">Parte del nombre del cliente; ignora mayúsculas, no tildes.</param>
/// <param name="Estado">Uno de los estados válidos; ignora mayúsculas.</param>
/// <param name="Desde">Inicio del rango (inclusivo), formato yyyy-MM-dd. Va junto con <paramref name="Hasta"/>.</param>
/// <param name="Hasta">Fin del rango (inclusivo), formato yyyy-MM-dd. Va junto con <paramref name="Desde"/>.</param>
/// <param name="Pagina">Número de página, desde 1. Por defecto 1.</param>
/// <param name="Tamano">Pedidos por página, de 1 a 100. Por defecto 20.</param>
public sealed record ConsultaPedidos(
    string? ClienteId = null,
    string? Cliente = null,
    string? Estado = null,
    string? Desde = null,
    string? Hasta = null,
    int? Pagina = null,
    int? Tamano = null);
