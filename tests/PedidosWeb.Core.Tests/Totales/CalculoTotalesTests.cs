using PedidosWeb.Core.Totales;

namespace PedidosWeb.Core.Tests.Totales;

/// <summary>
/// specs/HU-03-calculo-totales/spec.md: el C# calcula igual que el legado, peso a peso,
/// en los 60 casos de migracion/golden-master.csv.
/// </summary>
public class CalculoTotalesTests
{
    public static TheoryData<CasoGoldenMaster> Casos()
    {
        var datos = new TheoryData<CasoGoldenMaster>();
        foreach (var caso in GoldenMasterCsv.Leer())
        {
            datos.Add(caso);
        }
        return datos;
    }

    // CA-01
    [Theory]
    [MemberData(nameof(Casos))]
    public void Calcular_CasoGoldenMaster_IgualaAlLegado(CasoGoldenMaster caso)
    {
        var resultado = CalculoTotales.Calcular(
            caso.Lineas, caso.EsVip, caso.Region, caso.FleteBase, caso.TasaIva);

        Assert.True(caso.Descuento == resultado.Descuento,
            $"{caso}: descuento esperado {caso.Descuento}, obtenido {resultado.Descuento}");
        Assert.True(caso.Flete == resultado.Flete,
            $"{caso}: flete esperado {caso.Flete}, obtenido {resultado.Flete}");
        Assert.True(caso.Iva == resultado.Iva,
            $"{caso}: IVA esperado {caso.Iva}, obtenido {resultado.Iva}");
        Assert.True(caso.Total == resultado.Total,
            $"{caso}: total esperado {caso.Total}, obtenido {resultado.Total}");
    }

    // CA-01 · con R05 corregido la columna total suma 58.202.144 (el legado sumaba 58.202.135).
    [Fact]
    public void CalcularTodos_GoldenMaster_SumaTotal58202144()
    {
        var suma = GoldenMasterCsv.Leer()
            .Sum(c => CalculoTotales.Calcular(c.Lineas, c.EsVip, c.Region, c.FleteBase, c.TasaIva).Total);

        Assert.Equal(58_202_144m, suma);
    }

    // R05 · caso 6 del golden master: 515.350 × 5 % = 25.767,5 → 25.768 (el legado daba 25.767).
    [Fact]
    public void Calcular_DescuentoConMedioPeso_RedondeaHaciaArriba()
    {
        var caso6 = GoldenMasterCsv.Leer().Single(c => c.Caso == 6);

        var resultado = CalculoTotales.Calcular(
            caso6.Lineas, caso6.EsVip, caso6.Region, caso6.FleteBase, caso6.TasaIva);

        Assert.Equal(515_350m, resultado.BaseDescuento);
        Assert.Equal(25_768m, resultado.Descuento);
    }
}
