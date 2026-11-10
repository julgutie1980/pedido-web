namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Una página del listado de pedidos.
/// </summary>
/// <param name="Pagina">Número de la página devuelta.</param>
/// <param name="Tamano">Pedidos por página usados en la consulta.</param>
/// <param name="Total">Cantidad de pedidos que cumplen los filtros, antes de paginar.</param>
/// <param name="Pedidos">Pedidos de esta página, ordenados por fecha descendente y, a igual fecha, por Id descendente.</param>
public sealed record PaginaPedidos(int Pagina, int Tamano, int Total, IReadOnlyList<Pedido> Pedidos);
