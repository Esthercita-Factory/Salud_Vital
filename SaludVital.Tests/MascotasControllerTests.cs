using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SaludVital.Controllers;
using SaludVital.Models;
using SaludVital.Repositories;

namespace SaludVital.Tests;

public class MascotasControllerTests
{
    private readonly RepositorioMascotasEnMemoria _repositorio = new();

    [Fact]
    public void Index_DevuelveLaVistaConTodasLasMascotas()
    {
        var controlador = CrearControlador();

        var resultado = controlador.Index(null) as ViewResult;

        Assert.NotNull(resultado);
        var mascotas = Assert.IsType<List<Mascota>>(resultado.Model);
        Assert.Equal(10, mascotas.Count);
    }

    [Fact]
    public void Index_ConBusqueda_DevuelveSoloLosResultadosFiltrados()
    {
        var controlador = CrearControlador();

        var resultado = controlador.Index("firulais") as ViewResult;

        var mascotas = Assert.IsType<List<Mascota>>(resultado!.Model);
        Assert.Single(mascotas);
        Assert.Equal("Firulais", mascotas[0].Nombre);
    }

    [Fact]
    public void Detalles_ConIdExistente_DevuelveLaVistaConLaMascota()
    {
        var mascota = _repositorio.ObtenerTodas().First();
        var controlador = CrearControlador();

        var resultado = controlador.Detalles(mascota.Id) as ViewResult;

        Assert.NotNull(resultado);
        Assert.Equal(mascota.Id, Assert.IsType<Mascota>(resultado.Model).Id);
    }

    [Fact]
    public void Detalles_ConIdInexistente_DevuelveNotFound()
    {
        var controlador = CrearControlador();

        var resultado = controlador.Detalles(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(resultado);
    }

    [Fact]
    public void Crear_PostConModeloValido_RedirigeALaFicha()
    {
        var controlador = CrearControlador();
        var mascota = UnaMascota();

        var resultado = controlador.Crear(mascota) as RedirectToActionResult;

        Assert.NotNull(resultado);
        Assert.Equal(nameof(MascotasController.Detalles), resultado.ActionName);
        Assert.Equal(mascota.Id, resultado.RouteValues!["id"]);
    }

    [Fact]
    public void Crear_PostConModeloInvalido_DevuelveLaVistaConElModelo()
    {
        var controlador = CrearControlador();
        controlador.ModelState.AddModelError("Nombre", "El nombre es obligatorio.");
        var mascota = UnaMascota();

        var resultado = controlador.Crear(mascota) as ViewResult;

        Assert.NotNull(resultado);
        Assert.Same(mascota, resultado.Model);
    }

    [Fact]
    public void Editar_PostConModeloValido_ModificaYRedirigeALaFicha()
    {
        var mascota = _repositorio.ObtenerTodas().First();
        mascota.Nombre = "Renombrada";
        var controlador = CrearControlador();

        var resultado = controlador.Editar(mascota.Id, mascota) as RedirectToActionResult;

        Assert.NotNull(resultado);
        Assert.Equal(nameof(MascotasController.Detalles), resultado.ActionName);
        Assert.Equal("Renombrada", _repositorio.BuscarPorId(mascota.Id)!.Nombre);
    }

    [Fact]
    public void Editar_PostConModeloInvalido_DevuelveLaVista()
    {
        var mascota = _repositorio.ObtenerTodas().First();
        var controlador = CrearControlador();
        controlador.ModelState.AddModelError("Nombre", "El nombre es obligatorio.");

        var resultado = controlador.Editar(mascota.Id, mascota) as ViewResult;

        Assert.NotNull(resultado);
        Assert.Same(mascota, resultado.Model);
    }

    [Fact]
    public void Editar_PostConIdsDistintos_DevuelveBadRequest()
    {
        var mascota = _repositorio.ObtenerTodas().First();
        var controlador = CrearControlador();

        var resultado = controlador.Editar(Guid.NewGuid(), mascota);

        Assert.IsType<BadRequestResult>(resultado);
    }

    [Fact]
    public void Eliminar_PostConfirmaElBorrado_YRedirigeAlCatalogo()
    {
        var mascota = _repositorio.ObtenerTodas().First();
        var controlador = CrearControlador();

        var resultado = controlador.ConfirmarEliminar(mascota.Id) as RedirectToActionResult;

        Assert.NotNull(resultado);
        Assert.Equal(nameof(MascotasController.Index), resultado.ActionName);
        Assert.Null(_repositorio.BuscarPorId(mascota.Id));
    }

    [Fact]
    public void Eliminar_PostConIdInexistente_DevuelveNotFound()
    {
        var controlador = CrearControlador();

        var resultado = controlador.ConfirmarEliminar(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(resultado);
    }

    private MascotasController CrearControlador()
    {
        var controlador = new MascotasController(_repositorio)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
        controlador.TempData = new TempDataDictionary(controlador.ControllerContext.HttpContext, new FalsoTempDataProvider());
        return controlador;
    }

    private static Mascota UnaMascota()
    {
        return new Mascota
        {
            Id = Guid.NewGuid(),
            Nombre = "Nueva",
            Especie = "Perro",
            Raza = "Criollo",
            Sexo = Sexo.Macho,
            EdadEnMeses = 12,
            PesoEnKg = 10m,
            NombreDelDuenio = "Dueño de prueba",
            TelefonoDelDuenio = "3001234567"
        };
    }

    private sealed class FalsoTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();

        public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        {
        }
    }
}
