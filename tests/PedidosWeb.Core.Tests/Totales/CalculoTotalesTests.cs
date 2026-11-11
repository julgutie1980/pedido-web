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
}
