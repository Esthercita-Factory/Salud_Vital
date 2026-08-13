using SaludVital.Models;

namespace SaludVital.Tests;

public class MascotaValidacionTests
{
    [Fact]
    public void UnaMascotaValida_NoGeneraErrores()
    {
        var mascota = UnaMascotaValida();

        Assert.Empty(ValidadorDeModelos.ErroresDe(mascota));
    }

    [Fact]
    public void SinNombre_GeneraError()
    {
        var mascota = UnaMascotaValida();
        mascota.Nombre = "";

        var errores = ValidadorDeModelos.ErroresQueContienen(mascota, "obligatorio");

        Assert.Contains(errores, e => e.Contains("nombre", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void NombreConNumeros_GeneraError()
    {
        var mascota = UnaMascotaValida();
        mascota.Nombre = "Firulais123";

        Assert.Contains(ValidadorDeModelos.ErroresDe(mascota), e => e.Contains("números"));
    }

    [Fact]
    public void NombreDemasiadoCorto_GeneraError()
    {
        var mascota = UnaMascotaValida();
        mascota.Nombre = "A";

        Assert.Contains(ValidadorDeModelos.ErroresDe(mascota), e => e.Contains("entre 2 y 50"));
    }

    [Fact]
    public void EdadFueraDeRango_GeneraError()
    {
        var mascota = UnaMascotaValida();
        mascota.EdadEnMeses = 700;

        Assert.Contains(ValidadorDeModelos.ErroresDe(mascota), e => e.Contains("edad"));
    }

    [Fact]
    public void PesoCero_GeneraError()
    {
        var mascota = UnaMascotaValida();
        mascota.PesoEnKg = 0m;

        Assert.Contains(ValidadorDeModelos.ErroresDe(mascota), e => e.Contains("peso"));
    }

    [Fact]
    public void TelefonoConFormatoInvalido_GeneraError()
    {
        var mascota = UnaMascotaValida();
        mascota.TelefonoDelDuenio = "abc";

        Assert.Contains(ValidadorDeModelos.ErroresDe(mascota), e => e.Contains("teléfono", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void SinDuenio_GeneraError()
    {
        var mascota = UnaMascotaValida();
        mascota.NombreDelDuenio = "";

        Assert.Contains(ValidadorDeModelos.ErroresQueContienen(mascota, "obligatorio"),
            e => e.Contains("dueño", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void RazaConNumeros_GeneraError()
    {
        var mascota = UnaMascotaValida();
        mascota.Raza = "Criollo 2000";

        Assert.Contains(ValidadorDeModelos.ErroresDe(mascota), e => e.Contains("números"));
    }

    [Fact]
    public void NotasDemasiadoLargas_GeneranError()
    {
        var mascota = UnaMascotaValida();
        mascota.Notas = new string('x', 501);

        Assert.Contains(ValidadorDeModelos.ErroresDe(mascota), e => e.Contains("notas", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void NotasVacias_NoGeneranError()
    {
        var mascota = UnaMascotaValida();
        mascota.Notas = "";

        Assert.Empty(ValidadorDeModelos.ErroresDe(mascota));
    }

    [Fact]
    public void Normalizar_RecortaLosTextos()
    {
        var mascota = UnaMascotaValida();
        mascota.Nombre = "  Firulais  ";
        mascota.Notas = "  Con espacio  ";

        mascota.Normalizar();

        Assert.Equal("Firulais", mascota.Nombre);
        Assert.Equal("Con espacio", mascota.Notas);
    }

    private static Mascota UnaMascotaValida()
    {
        return new Mascota
        {
            Nombre = "Firulais",
            Especie = "Perro",
            Raza = "Criollo",
            Sexo = Sexo.Macho,
            EdadEnMeses = 36,
            PesoEnKg = 18.5m,
            NombreDelDuenio = "Carlos Gómez",
            TelefonoDelDuenio = "3001234567",
            EstaActivo = true
        };
    }
}
