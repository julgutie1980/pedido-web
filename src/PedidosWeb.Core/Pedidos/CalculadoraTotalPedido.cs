using PedidosWeb.Core.Lineas;

namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Calcula montos del pedido desde sus líneas (HU-02). Pesos chilenos (CLP), sin IVA.
/// IVA, descuentos y recargos quedan fuera de alcance hasta la Unidad 5.
/// </summary>
public static class CalculadoraTotalPedido
{
    /// <summary>Subtotal de una línea: <c>Cantidad × PrecioUnitario</c>.</summary>
    public static decimal Subtotal(LineaPedido linea) => linea.Cantidad * linea.PrecioUnitario;

    /// <summary>Suma de los subtotales de las líneas; 0 si no hay líneas.</summary>
    public static decimal CalcularTotal(IEnumerable<LineaPedido> lineas) => lineas.Sum(Subtotal);
}
