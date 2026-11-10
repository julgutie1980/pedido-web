namespace PedidosWeb.Core.Folios;

/// <summary>
/// Genera el folio de un pedido con el mismo formato del sistema legado.
/// Las reglas completas y los ejemplos están en el README, sección «La función del taller».
/// </summary>
public static class FolioFormatter
{
    /// <summary>
    /// Devuelve el folio con el formato PED-{SUCURSAL}-{NUMERO}, por ejemplo "PED-STGO-000123".
    /// </summary>
    /// <param name="numero">Número correlativo del pedido, entre 1 y 999999.</param>
    /// <param name="sucursal">Código de la sucursal: de 2 a 4 letras de la A a la Z.</param>
    public static string FormatearFolio(int numero, string sucursal)
    {
        if (numero < 1 || numero > 999999)
            throw new ArgumentOutOfRangeException(nameof(numero), numero, "El número debe estar entre 1 y 999999.");

        if (sucursal == null)
            throw new ArgumentNullException(nameof(sucursal), "La sucursal no puede ser nula.");

        if (string.IsNullOrWhiteSpace(sucursal))
            throw new ArgumentException("La sucursal no puede estar vacía.", nameof(sucursal));

        var codigo = sucursal.Trim().ToUpperInvariant();

        if (codigo.Length < 2 || codigo.Length > 4 || !codigo.All(c => c >= 'A' && c <= 'Z'))
            throw new ArgumentException("La sucursal debe tener entre 2 y 4 letras de la A a la Z.", nameof(sucursal));

        return $"PED-{codigo}-{numero:D6}";
    }
}
