namespace PedidosWeb.Core.Totales;

/// <summary>
/// Cálculo de totales del pedido migrado desde <c>CalcularTotales</c>
/// de <c>legado/src/UCalculoTotales.pas:36-37</c> (HU-03).
/// El contrato es <c>migracion/golden-master.csv</c>.
/// </summary>
public static class CalculoTotales
{
    /// <summary>Calcula subtotal, descuento, flete, neto, IVA y total de un pedido.</summary>
    /// <param name="lineas">Líneas del pedido.</param>
    /// <param name="esVip">Si el cliente es VIP (<c>TIPO = 'V'</c>).</param>
    /// <param name="region">Código de región del cliente.</param>
    /// <param name="fleteBase">Flete base de la región, en CLP.</param>
    /// <param name="tasaIva">Tasa de IVA como fracción (0.19).</param>
    public static ResultadoTotales Calcular(
        IReadOnlyList<LineaCalculo> lineas,
        bool esVip,
        int region,
        decimal fleteBase,
        decimal tasaIva)
        => throw new NotImplementedException();
}
