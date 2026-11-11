using System.Globalization;
using PedidosWeb.Core.Totales;

namespace PedidosWeb.Core.Tests.Totales;

/// <summary>Un caso de <c>migracion/golden-master.csv</c>: la entrada y lo que calcula hoy el legado.</summary>
public sealed record CasoGoldenMaster(
    int Caso,
    int Folio,
    bool EsVip,
    int Region,
    decimal FleteBase,
    decimal TasaIva,
    IReadOnlyList<LineaCalculo> Lineas,
    decimal Subtotal,
    decimal BaseDescuento,
    decimal PorcDescuento,
    decimal Descuento,
    decimal Flete,
    decimal Neto,
    decimal Iva,
    decimal Total)
{
    public override string ToString() => $"caso {Caso} · folio {Folio}";
}

/// <summary>Lee el golden master que el proyecto de tests copia a su carpeta de salida.</summary>
public static class GoldenMasterCsv
{
    public const int CasosEsperados = 60;

    private static readonly CultureInfo Invariante = CultureInfo.InvariantCulture;

    public static IReadOnlyList<CasoGoldenMaster> Leer()
    {
        var ruta = Path.Combine(AppContext.BaseDirectory, "golden-master.csv");
        var casos = File.ReadLines(ruta)
            .Skip(1)
            .Where(linea => !string.IsNullOrWhiteSpace(linea))
            .Select(ParsearFila)
            .ToList();

        if (casos.Count != CasosEsperados)
        {
            throw new InvalidDataException(
                $"El golden master debe tener {CasosEsperados} casos y tiene {casos.Count}. ¿Cambió el CSV?");
        }

        return casos;
    }

    private static CasoGoldenMaster ParsearFila(string fila)
    {
        var c = fila.Split(',');
        if (c.Length != 15)
        {
            throw new InvalidDataException($"Fila con {c.Length} columnas (se esperan 15): {fila}");
        }

        return new CasoGoldenMaster(
            Caso: int.Parse(c[0], Invariante),
            Folio: int.Parse(c[1], Invariante),
            EsVip: c[2] == "V",
            Region: int.Parse(c[3], Invariante),
            FleteBase: Dec(c[4]),
            TasaIva: Dec(c[5]),
            Lineas: ParsearLineas(c[6]),
            Subtotal: Dec(c[7]),
            BaseDescuento: Dec(c[8]),
            PorcDescuento: Dec(c[9]),
            Descuento: Dec(c[10]),
            Flete: Dec(c[11]),
            Neto: Dec(c[12]),
            Iva: Dec(c[13]),
            Total: Dec(c[14]));
    }

    /// <summary>Convierte <c>5x8210xBEB|30x5000xCON</c> en líneas de cálculo.</summary>
    private static IReadOnlyList<LineaCalculo> ParsearLineas(string texto) =>
        texto.Split('|')
            .Select(linea => linea.Split('x'))
            .Select(p => new LineaCalculo(int.Parse(p[0], Invariante), Dec(p[1]), p[2]))
            .ToList();

    private static decimal Dec(string valor) =>
        decimal.Parse(valor, NumberStyles.Number, Invariante);
}
