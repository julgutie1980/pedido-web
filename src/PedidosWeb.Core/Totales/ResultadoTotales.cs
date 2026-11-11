namespace PedidosWeb.Core.Totales;

/// <summary>
/// Resultado del cálculo de totales. Equivale a <c>TResultadoTotales</c>
/// de <c>legado/src/UCalculoTotales.pas:18-27</c>, con <c>decimal</c> en vez de <c>Double</c>.
/// Montos en pesos chilenos (CLP); <see cref="PorcDescuento"/> es una fracción (0.08 = 8 %).
/// </summary>
public sealed record ResultadoTotales(
    decimal Subtotal,
    decimal BaseDescuento,
    decimal PorcDescuento,
    decimal Descuento,
    decimal Flete,
    decimal Neto,
    decimal Iva,
    decimal Total);
