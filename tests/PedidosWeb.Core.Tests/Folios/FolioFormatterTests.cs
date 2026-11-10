using PedidosWeb.Core.Folios;

namespace PedidosWeb.Core.Tests.Folios;

public class FolioFormatterTests
{
    // Casos base del README (tres primeras filas de la tabla de ejemplos).
    [Theory]
    [InlineData(123, "STGO", "PED-STGO-000123")]
    [InlineData(1, "VAL", "PED-VAL-000001")]
    [InlineData(999999, "CO", "PED-CO-999999")]
    public void FormatearFolio_CasosBase_DevuelveElFolioEsperado(int numero, string sucursal, string esperado)
    {
        var folio = FolioFormatter.FormatearFolio(numero, sucursal);

        Assert.Equal(esperado, folio);
    }

    // R2: a la sucursal se le quitan los espacios del inicio y del final, y se pasa a mayúsculas.
    [Fact]
    public void FormatearFolio_SucursalConEspaciosYMinusculas_LaNormaliza()
    {
        var folio = FolioFormatter.FormatearFolio(45, " stgo ");

        Assert.Equal("PED-STGO-000045", folio);
    }

    // R1: el número debe estar entre 1 y 999999 (ambos incluidos).
    [Theory]
    [InlineData(0)]
    [InlineData(1000000)]
    [InlineData(-5)]
    public void FormatearFolio_NumeroFueraDeRango_LanzaArgumentOutOfRangeException(int numero)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => FolioFormatter.FormatearFolio(numero, "STGO"));
    }

    // R3: después de R2, la sucursal debe tener entre 2 y 4 letras de la A a la Z.
    // Vale ArgumentException o una derivada (por ejemplo, ArgumentNullException).
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("S")]
    [InlineData("SANTI")]
    [InlineData("ÑUÑO")]
    [InlineData("ST1")]
    public void FormatearFolio_SucursalInvalida_LanzaArgumentException(string? sucursal)
    {
        Assert.ThrowsAny<ArgumentException>(
            () => FolioFormatter.FormatearFolio(7, sucursal!));
    }

    // R5: si número y sucursal son inválidos a la vez, se valida primero el número.
    [Fact]
    public void FormatearFolio_NumeroYSucursalInvalidos_ValidaPrimeroElNumero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => FolioFormatter.FormatearFolio(0, "S"));
    }
}
