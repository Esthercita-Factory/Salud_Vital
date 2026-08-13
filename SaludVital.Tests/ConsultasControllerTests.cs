using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SaludVital.Controllers;
using SaludVital.Models;
using SaludVital.Repositories;

namespace SaludVital.Tests;

public class ConsultasControllerTests
{
    private readonly RepositorioMascotasEnMemoria _repositorioMascotas = new();
    private readonly RepositorioConsultasEnMemoria _repositorioConsultas;

    public ConsultasControllerTests()
    {
        _repositorioConsultas = new RepositorioConsultasEnMemoria(_repositorioMascotas);
    }

    [Fact]
    public void Crear_ConMascotaExistente_DevuelveLaVistaConLaConsultaPreparada()
    {
        var mascota = _repositorioMascotas.ObtenerTodas().First();
        var controlador = CrearControlador();

        var resultado = controlador.Crear(mascota.Id) as ViewResult;

        Assert.NotNull(resultado);
        var consulta = Assert.IsType<Consulta>(resultado.Model);
        Assert.Equal(mascota.Id, consulta.MascotaId);
    }

    [Fact]
    public void Crear_ConMascotaInexistente_DevuelveNotFound()
    {
        var controlador = CrearControlador();

        var resultado = controlador.Crear(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(resultado);
    }

    [Fact]
    public void Crear_PostConModeloValido_RegistraYRedirigeALaFicha()
    {
        var mascota = _repositorioMascotas.ObtenerTodas().First();
        var consulta = UnaConsulta(mascota.Id);
        var controlador = CrearControlador();

        var resultado = controlador.Crear(consulta) as RedirectToActionResult;

        Assert.NotNull(resultado);
        Assert.Equal(nameof(MascotasController.Detalles), resultado.ActionName);
        Assert.Single(_repositorioConsultas.ObtenerPorMascota(mascota.Id));
    }

    [Fact]
    public void Crear_PostConModeloInvalido_DevuelveLaVistaConElModelo()
    {
        var mascota = _repositorioMascotas.ObtenerTodas().First();
        var consulta = UnaConsulta(mascota.Id);
        var controlador = CrearControlador();
        controlador.ModelState.AddModelError("Motivo", "El motivo es obligatorio.");

        var resultado = controlador.Crear(consulta) as ViewResult;

        Assert.NotNull(resultado);
        Assert.Same(consulta, resultado.Model);
    }

    [Fact]
    public void Editar_PostConModeloValido_ModificaYRedirigeALaFicha()
    {
        var mascota = _repositorioMascotas.ObtenerTodas().First();
        var consulta = UnaConsulta(mascota.Id);
        _repositorioConsultas.Registrar(consulta);
        consulta.Diagnostico = "Sano";
        var controlador = CrearControlador();

        var resultado = controlador.Editar(consulta.Id, consulta) as RedirectToActionResult;

        Assert.NotNull(resultado);
        Assert.Equal(nameof(MascotasController.Detalles), resultado.ActionName);
        Assert.Equal("Sano", _repositorioConsultas.BuscarPorId(consulta.Id)!.Diagnostico);
    }

    [Fact]
    public void Editar_PostConIdsDistintos_DevuelveBadRequest()
    {
        var consulta = UnaConsulta(Guid.NewGuid());
        var controlador = CrearControlador();

        var resultado = controlador.Editar(Guid.NewGuid(), consulta);

        Assert.IsType<BadRequestResult>(resultado);
    }

    [Fact]
    public void Eliminar_PostConfirmaElBorrado_YRedirigeALaFichaDeLaMascota()
    {
        var mascota = _repositorioMascotas.ObtenerTodas().First();
        var consulta = UnaConsulta(mascota.Id);
        _repositorioConsultas.Registrar(consulta);
        var controlador = CrearControlador();

        var resultado = controlador.ConfirmarEliminar(consulta.Id) as RedirectToActionResult;

        Assert.NotNull(resultado);
        Assert.Equal(mascota.Id, resultado.RouteValues!["id"]);
        Assert.Empty(_repositorioConsultas.ObtenerPorMascota(mascota.Id));
    }

    [Fact]
    public void Eliminar_PostConIdInexistente_DevuelveNotFound()
    {
        var controlador = CrearControlador();

        var resultado = controlador.ConfirmarEliminar(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(resultado);
    }

    private ConsultasController CrearControlador()
    {
        var controlador = new ConsultasController(_repositorioConsultas, _repositorioMascotas)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
        controlador.TempData = new TempDataDictionary(controlador.ControllerContext.HttpContext, new FalsoTempDataProvider());
        return controlador;
    }

    private static Consulta UnaConsulta(Guid mascotaId)
    {
        return new Consulta
        {
            Id = Guid.NewGuid(),
            MascotaId = mascotaId,
            Fecha = DateTime.Today,
            Motivo = "Chequeo anual"
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
