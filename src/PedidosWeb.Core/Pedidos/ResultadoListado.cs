namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Resultado de listar pedidos: una página válida o un mensaje de error de validación.
/// </summary>
public sealed class ResultadoListado
{
    private ResultadoListado(PaginaPedidos? pagina, string? error)
    {
        Pagina = pagina;
        Error = error;
    }

    /// <summary>La página de pedidos; null si la consulta no es válida.</summary>
    public PaginaPedidos? Pagina { get; }

    /// <summary>Mensaje del error de validación; null si la consulta es válida.</summary>
    public string? Error { get; }

    /// <summary>True si la consulta es válida y hay página.</summary>
    public bool EsValido => Error is null;

    public static ResultadoListado Ok(PaginaPedidos pagina) => new(pagina, null);

    public static ResultadoListado Fallo(string error) => new(null, error);
}
