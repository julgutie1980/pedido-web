namespace PedidosWeb.Core.Totales;

/// <summary>
/// Línea que entra al cálculo de totales. Equivale a <c>TLineaCalculo</c>
/// de <c>legado/src/UCalculoTotales.pas:10-14</c>, con <c>decimal</c> en vez de <c>Double</c>.
/// </summary>
/// <param name="Cantidad">Unidades pedidas.</param>
/// <param name="PrecioUnitario">Precio por unidad en pesos chilenos (CLP).</param>
/// <param name="Categoria">Código de categoría del producto (por ejemplo <c>BEB</c> u <c>OFE</c>).</param>
public sealed record LineaCalculo(int Cantidad, decimal PrecioUnitario, string Categoria);
