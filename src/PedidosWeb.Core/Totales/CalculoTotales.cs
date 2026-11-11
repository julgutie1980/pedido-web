namespace PedidosWeb.Core.Totales;

/// <summary>
/// Cálculo de totales del pedido migrado desde <c>CalcularTotales</c>
/// de <c>legado/src/UCalculoTotales.pas:108-122</c> (HU-03).
/// Replica el legado <b>defectos incluidos</b>; el contrato es <c>migracion/golden-master.csv</c>.
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
    {
        // UCalculoTotales.pas:111-121, en el mismo orden.
        var subtotal = Subtotal(lineas);
        var baseDescuento = BaseDescuento(lineas);
        var porcDescuento = PorcentajeDescuento(baseDescuento, esVip);
        var descuento = RedondearAlPeso(baseDescuento * porcDescuento);
        var neto = subtotal - descuento;
        var flete = lineas.Count == 0 ? 0m : Flete(fleteBase, region);
        var iva = Iva(neto, flete, tasaIva);
        var total = neto + flete + iva;

        return new ResultadoTotales(subtotal, baseDescuento, porcDescuento, descuento, flete, neto, iva, total);
    }

    // UCalculoTotales.pas:48-51 · Calc3. R05: suma 0,49 y trunca (x,50 baja). Se replica tal cual.
    private static decimal RedondearAlPeso(decimal valor) => Math.Truncate(valor + 0.49m);

    // UCalculoTotales.pas:53-60
    private static decimal Subtotal(IReadOnlyList<LineaCalculo> lineas) =>
        lineas.Sum(l => l.Cantidad * l.PrecioUnitario);

    // UCalculoTotales.pas:63-73 · la categoría OFE no entra en la base del descuento.
    private static decimal BaseDescuento(IReadOnlyList<LineaCalculo> lineas) =>
        lineas
            .Where(l => l.Categoria.Trim().ToUpperInvariant() != "OFE")
            .Sum(l => l.Cantidad * l.PrecioUnitario);

    // UCalculoTotales.pas:75-83 · umbrales con > estricto.
    private static decimal PorcentajeVolumen(decimal baseDescuento) =>
        baseDescuento > 1_000_000m ? 0.08m
        : baseDescuento > 500_000m ? 0.05m
        : 0m;

    // UCalculoTotales.pas:85-92 · VIP suma 3 puntos; tope de 10 %.
    private static decimal PorcentajeDescuento(decimal baseDescuento, bool esVip)
    {
        var porcentaje = PorcentajeVolumen(baseDescuento);
        if (esVip)
        {
            porcentaje += 0.03m;
        }
        return porcentaje > 0.10m ? 0.10m : porcentaje;
    }

    // UCalculoTotales.pas:95-101 · regiones 11 y 12 pagan 15 % más.
    private static decimal Flete(decimal fleteBase, int region) =>
        region is 11 or 12
            ? RedondearAlPeso(fleteBase * 1.15m)
            : RedondearAlPeso(fleteBase);

    // UCalculoTotales.pas:103-106
    private static decimal Iva(decimal neto, decimal flete, decimal tasa) =>
        RedondearAlPeso((neto + flete) * tasa);
}
