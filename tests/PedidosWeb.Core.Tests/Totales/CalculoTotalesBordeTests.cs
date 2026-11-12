using PedidosWeb.Core.Totales;

namespace PedidosWeb.Core.Tests.Totales;

/// <summary>
/// U6 · Etapa 1: casos de borde que migracion/golden-master.csv no cubre.
/// Cada valor esperado está calculado a mano; ninguno se compara con el propio resultado.
/// </summary>
public class CalculoTotalesBordeTests
{
    // Un pedido vacío no paga flete, ni siquiera en la región 12 con recargo.
    [Fact]
    public void Calcular_SinLineas_TodoEnCero()
    {
        var resultado = CalculoTotales.Calcular([], esVip: true, region: 12, fleteBase: 16_490m, tasaIva: 0.19m);

        Assert.Equal(0m, resultado.Subtotal);
        Assert.Equal(0m, resultado.Descuento);
        Assert.Equal(0m, resultado.Flete);
        Assert.Equal(0m, resultado.Iva);
        Assert.Equal(0m, resultado.Total);
    }

    // " ofe " (minúsculas y espacios) sigue siendo oferta: 600.000 de base → 5 % = 30.000.
    // Sin Trim/ToUpperInvariant la base sería 1.100.000 y el descuento 88.000.
    [Fact]
    public void Calcular_CategoriaOfeEnMinusculasConEspacios_NoEntraEnBaseDescuento()
    {
        LineaCalculo[] lineas =
        [
            new(10, 60_000m, "ABA"),
            new(10, 50_000m, " ofe "),
        ];

        var resultado = CalculoTotales.Calcular(lineas, esVip: false, region: 13, fleteBase: 3_500m, tasaIva: 0.19m);

        Assert.Equal(600_000m, resultado.BaseDescuento);
        Assert.Equal(30_000m, resultado.Descuento);
    }

    // El IVA usa la tasa recibida, no 0,19 fijo: (100.000 + 3.500) × 10 % = 10.350.
    [Fact]
    public void Calcular_TasaIvaDistinta_UsaLaTasaRecibida()
    {
        LineaCalculo[] lineas = [new(1, 100_000m, "ABA")];

        var resultado = CalculoTotales.Calcular(lineas, esVip: false, region: 13, fleteBase: 3_500m, tasaIva: 0.10m);

        Assert.Equal(10_350m, resultado.Iva);
        Assert.Equal(113_850m, resultado.Total);
    }

    // VIP con solo ofertas: base 0, así que el 3 % VIP no descuenta nada.
    // (71.980 + 3.500) × 19 % = 14.341,2 → 14.341; total 89.821.
    [Fact]
    public void Calcular_VipSoloConOfertas_DescuentoCero()
    {
        LineaCalculo[] lineas = [new(2, 35_990m, "OFE")];

        var resultado = CalculoTotales.Calcular(lineas, esVip: true, region: 13, fleteBase: 3_500m, tasaIva: 0.19m);

        Assert.Equal(0.03m, resultado.PorcDescuento);
        Assert.Equal(0m, resultado.Descuento);
        Assert.Equal(14_341m, resultado.Iva);
        Assert.Equal(89_821m, resultado.Total);
    }

    // Bajo el medio peso redondea hacia abajo: 600.008 × 5 % = 30.000,4 → 30.000.
    [Fact]
    public void Calcular_DescuentoBajoMedioPeso_RedondeaHaciaAbajo()
    {
        LineaCalculo[] lineas = [new(1, 600_008m, "ABA")];

        var resultado = CalculoTotales.Calcular(lineas, esVip: false, region: 13, fleteBase: 3_500m, tasaIva: 0.19m);

        Assert.Equal(30_000m, resultado.Descuento);
    }
}
