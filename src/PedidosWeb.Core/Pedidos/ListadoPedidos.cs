using System.Globalization;

namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// HU-01 · Lista pedidos filtrando por cliente, estado y rango de fechas, con paginación.
/// Reglas en specs/HU-01-listar-pedidos/spec.md.
/// </summary>
public sealed class ListadoPedidos
{
    public const int TamanoPorDefecto = 20;
    public const int TamanoMaximo = 100;
    public const string FormatoFecha = "yyyy-MM-dd";

    /// <summary>RN7: estados válidos para filtrar.</summary>
    public static readonly IReadOnlyList<string> EstadosValidos =
        new[] { "Pendiente", "Confirmado", "Despachado", "Entregado", "Anulado" };

    private readonly IPedidoRepositorio _repositorio;

    public ListadoPedidos(IPedidoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public ResultadoListado Listar(ConsultaPedidos consulta)
    {
        // RN16: formato de fecha.
        if (!TryLeerFecha(consulta.Desde, "desde", out var desde, out var error) ||
            !TryLeerFecha(consulta.Hasta, "hasta", out var hasta, out error))
            return ResultadoListado.Fallo(error!);

        // RN5: desde y hasta van juntos.
        if (desde.HasValue != hasta.HasValue)
            return ResultadoListado.Fallo("Debe enviar 'desde' y 'hasta' juntos.");

        // RN8: fechas invertidas.
        if (desde > hasta)
            return ResultadoListado.Fallo(
                $"La fecha 'desde' ({consulta.Desde}) no puede ser mayor que 'hasta' ({consulta.Hasta}).");

        // RN7 y RN11: estado válido, sin distinguir mayúsculas.
        if (consulta.Estado is not null &&
            !EstadosValidos.Contains(consulta.Estado, StringComparer.OrdinalIgnoreCase))
            return ResultadoListado.Fallo(
                $"El estado '{consulta.Estado}' no es válido. Estados válidos: {string.Join(", ", EstadosValidos)}.");

        // RN9 y RN15: paginación.
        var pagina = consulta.Pagina ?? 1;
        var tamano = consulta.Tamano ?? TamanoPorDefecto;
        if (pagina < 1)
            return ResultadoListado.Fallo("La 'pagina' debe ser mayor o igual a 1.");
        if (tamano < 1 || tamano > TamanoMaximo)
            return ResultadoListado.Fallo($"El 'tamano' debe estar entre 1 y {TamanoMaximo}.");

        // RN1, RN2, RN3, RN4, RN10, RN12: filtros opcionales combinados con Y.
        IEnumerable<Pedido> pedidos = _repositorio.ObtenerTodos();

        if (consulta.ClienteId is not null)
            pedidos = pedidos.Where(p => string.Equals(p.ClienteId, consulta.ClienteId, StringComparison.Ordinal));

        if (consulta.Cliente is not null)
            pedidos = pedidos.Where(p => p.ClienteNombre.Contains(consulta.Cliente, StringComparison.OrdinalIgnoreCase));

        if (consulta.Estado is not null)
            pedidos = pedidos.Where(p => string.Equals(p.Estado, consulta.Estado, StringComparison.OrdinalIgnoreCase));

        if (desde.HasValue && hasta.HasValue)
            pedidos = pedidos.Where(p => p.Fecha >= desde.Value && p.Fecha <= hasta.Value);

        // RN6: fecha descendente y, a igual fecha, Id descendente.
        var filtrados = pedidos
            .OrderByDescending(p => p.Fecha)
            .ThenByDescending(p => p.Id)
            .ToList();

        // RN14 y RN15: total antes de paginar; una página más allá de la última viene vacía.
        var paginaPedidos = filtrados
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToList();

        return ResultadoListado.Ok(new PaginaPedidos(pagina, tamano, filtrados.Count, paginaPedidos));
    }

    private static bool TryLeerFecha(string? texto, string nombre, out DateOnly? fecha, out string? error)
    {
        fecha = null;
        error = null;

        if (texto is null)
            return true;

        if (DateOnly.TryParseExact(texto, FormatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out var valor))
        {
            fecha = valor;
            return true;
        }

        error = $"La fecha '{nombre}' ({texto}) no es válida. Use el formato {FormatoFecha}.";
        return false;
    }
}
