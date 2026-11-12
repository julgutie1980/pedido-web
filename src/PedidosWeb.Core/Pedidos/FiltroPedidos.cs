using System.Globalization;

namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// HU-03 · Filtra pedidos emitidos entre dos fechas, con ambos límites inclusive.
/// Reglas en specs/HU-03-pedidos-por-fecha/spec.md.
/// </summary>
public static class FiltroPedidos
{
    private const string FormatoFecha = "yyyy-MM-dd";

    /// <summary>
    /// Devuelve, en el mismo orden de entrada, los pedidos cuya <see cref="Pedido.Fecha"/>
    /// está entre <paramref name="desde"/> y <paramref name="hasta"/> (CA-01, CA-02 y CA-03).
    /// </summary>
    /// <exception cref="ArgumentNullException">Si <paramref name="pedidos"/> es nulo.</exception>
    /// <exception cref="ArgumentException">Si <paramref name="desde"/> es mayor que <paramref name="hasta"/> (CA-04).</exception>
    public static IReadOnlyList<Pedido> EntreFechas(IEnumerable<Pedido> pedidos, DateOnly desde, DateOnly hasta)
    {
        ArgumentNullException.ThrowIfNull(pedidos);

        if (desde > hasta)
            throw new ArgumentException(
                $"La fecha 'desde' ({Formatear(desde)}) no puede ser mayor que 'hasta' ({Formatear(hasta)}).",
                nameof(desde));

        return pedidos
            .Where(p => p.Fecha >= desde && p.Fecha <= hasta)
            .ToList();
    }

    private static string Formatear(DateOnly fecha) =>
        fecha.ToString(FormatoFecha, CultureInfo.InvariantCulture);
}
