namespace PedidosWeb.Core.Clientes;

/// <summary>
/// Consulta de clientes: la ficha para la pantalla y si el cliente está activo.
/// </summary>
public class ClientesService
{
    private readonly List<Cliente> _clientes;

    public ClientesService() : this(ClientesDeEjemplo.Crear()) { }

    public ClientesService(List<Cliente> clientes)
    {
        _clientes = clientes;
    }

    /// <summary>
    /// Ficha del cliente para la pantalla: «Nombre | RUT | Zona | Estado».
    /// Devuelve "" si el cliente no existe.
    /// </summary>
    public string ObtenerFicha(string id)
    {
        Cliente? cliente = BuscarPorId(id);
        if (cliente == null) return "";
        string rut = cliente.Rut.Replace(".", "").Replace(" ", "").ToUpper();
        string zona = ZonaDeRegion(cliente.Region);
        string estado = cliente.Activo ? "Activo" : "Inactivo";
        return cliente.Nombre + " | " + rut + " | " + zona + " | " + estado;
    }

    /// <summary>True solo si el cliente existe y está activo.</summary>
    public bool EstaActivo(string id)
    {
        Cliente? cliente = BuscarPorId(id);
        if (cliente == null) return false;
        return cliente.Activo;
    }

    /// <summary>Primer cliente con ese id exacto, o null si no existe.</summary>
    private Cliente? BuscarPorId(string id) => _clientes.FirstOrDefault(c => c.Id == id);

    /// <summary>Zona comercial según la región del cliente.</summary>
    private static string ZonaDeRegion(string region) => region switch
    {
        "RM" or "V" => "Zona centro",
        "VIII" => "Zona sur",
        "II" => "Zona norte",
        _ => "Sin zona",
    };
}
