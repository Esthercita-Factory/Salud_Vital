using SaludVital.Models;
using SaludVital.Repositories;

namespace SaludVital.Tests;

public class RepositorioMascotasEnMemoriaTests
{
    private readonly RepositorioMascotasEnMemoria _repositorio = new();

    [Fact]
    public void AlCrearElRepositorio_SeCarganLasMascotasDeEjemplo()
    {
        Assert.Equal(10, _repositorio.ObtenerTodas().Count);
    }

    [Fact]
    public void BuscarPorId_DevuelveLaMascotaCorrecta()
    {
        var esperada = _repositorio.ObtenerTodas().First();

        var obtenida = _repositorio.BuscarPorId(esperada.Id);

        Assert.NotNull(obtenida);
        Assert.Equal(esperada.Id, obtenida.Id);
        Assert.Equal(esperada.Nombre, obtenida.Nombre);
    }

    [Fact]
    public void BuscarPorId_ConIdInexistente_DevuelveNull()
    {
        var obtenida = _repositorio.BuscarPorId(Guid.NewGuid());

        Assert.Null(obtenida);
    }

    [Fact]
    public void Registrar_AgregaLaMascotaAlRepositorio()
    {
        var mascota = UnaMascota("Nueva", "Perro", "Criollo");

        _repositorio.Registrar(mascota);

        Assert.Equal(11, _repositorio.ObtenerTodas().Count);
        Assert.NotNull(_repositorio.BuscarPorId(mascota.Id));
    }

    [Fact]
    public void Registrar_RespetaElIdAsignadoALaMascota()
    {
        var mascota = UnaMascota("Nueva", "Gato", "Criollo");

        _repositorio.Registrar(mascota);

        Assert.Equal(mascota.Id, _repositorio.BuscarPorId(mascota.Id)!.Id);
    }

    [Fact]
    public void Actualizar_ModificaLosDatosDeLaMascota()
    {
        var mascota = _repositorio.ObtenerTodas().First();
        mascota.Nombre = "Renombrada";
        mascota.PesoEnKg = 99.9m;

        _repositorio.Actualizar(mascota);

        var actualizada = _repositorio.BuscarPorId(mascota.Id);
        Assert.Equal("Renombrada", actualizada!.Nombre);
        Assert.Equal(99.9m, actualizada.PesoEnKg);
    }

    [Fact]
    public void Actualizar_ConIdInexistente_LanzaExcepcion()
    {
        var mascota = UnaMascota("Fantasma", "Perro", "Criollo");
        mascota.Id = Guid.NewGuid();

        Assert.Throws<InvalidOperationException>(() => _repositorio.Actualizar(mascota));
    }

    [Fact]
    public void Eliminar_QuitaLaMascotaDelRepositorio()
    {
        var mascota = _repositorio.ObtenerTodas().First();

        var eliminada = _repositorio.Eliminar(mascota.Id);

        Assert.True(eliminada);
        Assert.Equal(9, _repositorio.ObtenerTodas().Count);
        Assert.Null(_repositorio.BuscarPorId(mascota.Id));
    }

    [Fact]
    public void Eliminar_ConIdInexistente_DevuelveFalso()
    {
        var eliminada = _repositorio.Eliminar(Guid.NewGuid());

        Assert.False(eliminada);
        Assert.Equal(10, _repositorio.ObtenerTodas().Count);
    }

    [Fact]
    public void ObtenerTodas_ConBusquedaVacia_DevuelveTodas()
    {
        Assert.Equal(10, _repositorio.ObtenerTodas("").Count);
        Assert.Equal(10, _repositorio.ObtenerTodas("  ").Count);
        Assert.Equal(10, _repositorio.ObtenerTodas(null).Count);
    }

    [Fact]
    public void ObtenerTodas_FiltraPorNombre()
    {
        var resultados = _repositorio.ObtenerTodas("firulais");

        Assert.Single(resultados);
        Assert.Equal("Firulais", resultados[0].Nombre);
    }

    [Fact]
    public void ObtenerTodas_FiltraPorRazaOEspecie()
    {
        var porRaza = _repositorio.ObtenerTodas("labrador");
        var porEspecie = _repositorio.ObtenerTodas("gato");

        Assert.Single(porRaza);
        Assert.Equal(3, porEspecie.Count);
    }

    [Fact]
    public void ObtenerTodas_ConBusquedaSinResultados_DevuelveListaVacia()
    {
        Assert.Empty(_repositorio.ObtenerTodas("unicornio"));
    }

    private static Mascota UnaMascota(string nombre, string especie, string raza)
    {
        return new Mascota
        {
            Id = Guid.NewGuid(),
            Nombre = nombre,
            Especie = especie,
            Raza = raza,
            Sexo = Sexo.Macho,
            EdadEnMeses = 12,
            PesoEnKg = 10m,
            NombreDelDuenio = "Dueño de prueba",
            TelefonoDelDuenio = "3001234567"
        };
    }
}
