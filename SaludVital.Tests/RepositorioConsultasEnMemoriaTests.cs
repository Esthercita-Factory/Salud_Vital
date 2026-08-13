using SaludVital.Models;
using SaludVital.Repositories;

namespace SaludVital.Tests;

public class RepositorioConsultasEnMemoriaTests
{
    private readonly RepositorioMascotasEnMemoria _repositorioMascotas = new();
    private readonly RepositorioConsultasEnMemoria _repositorioConsultas;

    public RepositorioConsultasEnMemoriaTests()
    {
        _repositorioConsultas = new RepositorioConsultasEnMemoria(_repositorioMascotas);
    }

    [Fact]
    public void ObtenerPorMascota_DevuelveLasConsultasDeEsaMascota()
    {
        var mascota = _repositorioMascotas.ObtenerTodas().First();
        var consulta1 = UnaConsulta(mascota.Id, "Chequeo");
        var consulta2 = UnaConsulta(mascota.Id, "Vacunación");
        _repositorioConsultas.Registrar(consulta1);
        _repositorioConsultas.Registrar(consulta2);

        var consultas = _repositorioConsultas.ObtenerPorMascota(mascota.Id);

        Assert.Equal(2, consultas.Count);
    }

    [Fact]
    public void ObtenerPorMascota_ConIdInexistente_DevuelveListaVacia()
    {
        var consultas = _repositorioConsultas.ObtenerPorMascota(Guid.NewGuid());

        Assert.Empty(consultas);
    }

    [Fact]
    public void ObtenerPorMascota_OrdenaDeLaMasRecienteALaMasAntigua()
    {
        var mascota = _repositorioMascotas.ObtenerTodas().First();
        _repositorioConsultas.Registrar(UnaConsulta(mascota.Id, "Primera", new DateTime(2025, 1, 1)));
        _repositorioConsultas.Registrar(UnaConsulta(mascota.Id, "Segunda", new DateTime(2026, 1, 1)));

        var consultas = _repositorioConsultas.ObtenerPorMascota(mascota.Id);

        Assert.Equal("Segunda", consultas[0].Motivo);
    }

    [Fact]
    public void Registrar_AgregaLaConsultaALaMascota()
    {
        var mascota = _repositorioMascotas.ObtenerTodas().First();
        var consulta = UnaConsulta(mascota.Id, "Fiebre");

        _repositorioConsultas.Registrar(consulta);

        Assert.Contains(consulta, _repositorioMascotas.BuscarPorId(mascota.Id)!.Consultas);
    }

    [Fact]
    public void Registrar_ConMascotaInexistente_LanzaExcepcion()
    {
        var consulta = UnaConsulta(Guid.NewGuid(), "Fiebre");

        Assert.Throws<InvalidOperationException>(() => _repositorioConsultas.Registrar(consulta));
    }

    [Fact]
    public void BuscarPorId_DevuelveLaConsultaCorrecta()
    {
        var mascota = _repositorioMascotas.ObtenerTodas().First();
        var consulta = UnaConsulta(mascota.Id, "Vacunación");
        _repositorioConsultas.Registrar(consulta);

        var obtenida = _repositorioConsultas.BuscarPorId(consulta.Id);

        Assert.NotNull(obtenida);
        Assert.Equal("Vacunación", obtenida.Motivo);
    }

    [Fact]
    public void Actualizar_ModificaLosDatosDeLaConsulta()
    {
        var mascota = _repositorioMascotas.ObtenerTodas().First();
        var consulta = UnaConsulta(mascota.Id, "Chequeo");
        _repositorioConsultas.Registrar(consulta);
        consulta.Diagnostico = "Sano";

        _repositorioConsultas.Actualizar(consulta);

        Assert.Equal("Sano", _repositorioConsultas.BuscarPorId(consulta.Id)!.Diagnostico);
    }

    [Fact]
    public void Actualizar_ConIdInexistente_LanzaExcepcion()
    {
        var consulta = UnaConsulta(Guid.NewGuid(), "Fantasma");
        consulta.Id = Guid.NewGuid();

        Assert.Throws<InvalidOperationException>(() => _repositorioConsultas.Actualizar(consulta));
    }

    [Fact]
    public void Eliminar_QuitaLaConsultaDeLaMascota()
    {
        var mascota = _repositorioMascotas.ObtenerTodas().First();
        var consulta = UnaConsulta(mascota.Id, "Chequeo");
        _repositorioConsultas.Registrar(consulta);

        var eliminada = _repositorioConsultas.Eliminar(consulta.Id);

        Assert.True(eliminada);
        Assert.DoesNotContain(consulta, _repositorioMascotas.BuscarPorId(mascota.Id)!.Consultas);
    }

    [Fact]
    public void Eliminar_ConIdInexistente_DevuelveFalso()
    {
        Assert.False(_repositorioConsultas.Eliminar(Guid.NewGuid()));
    }

    private static Consulta UnaConsulta(Guid mascotaId, string motivo, DateTime? fecha = null)
    {
        return new Consulta
        {
            Id = Guid.NewGuid(),
            MascotaId = mascotaId,
            Fecha = fecha ?? DateTime.Today,
            Motivo = motivo
        };
    }
}
