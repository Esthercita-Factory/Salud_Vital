using SaludVital.Models;

namespace SaludVital.Tests;

public class ConsultaValidacionTests
{
    [Fact]
    public void UnaConsultaValida_NoGeneraErrores()
    {
        var consulta = UnaConsultaValida();

        Assert.Empty(ValidadorDeModelos.ErroresDe(consulta));
    }

    [Fact]
    public void SinMotivo_GeneraError()
    {
        var consulta = UnaConsultaValida();
        consulta.Motivo = "";

        Assert.Contains(ValidadorDeModelos.ErroresDe(consulta), e => e.Contains("motivo"));
    }

    [Fact]
    public void MotivoDemasiadoCorto_GeneraError()
    {
        var consulta = UnaConsultaValida();
        consulta.Motivo = "Al";

        Assert.Contains(ValidadorDeModelos.ErroresDe(consulta), e => e.Contains("entre 3 y 200"));
    }

    [Fact]
    public void DiagnosticoDemasiadoLargo_GeneraError()
    {
        var consulta = UnaConsultaValida();
        consulta.Diagnostico = new string('x', 501);

        Assert.Contains(ValidadorDeModelos.ErroresDe(consulta), e => e.Contains("diagnóstico", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void TratamientoDemasiadoLargo_GeneraError()
    {
        var consulta = UnaConsultaValida();
        consulta.Tratamiento = new string('x', 501);

        Assert.Contains(ValidadorDeModelos.ErroresDe(consulta), e => e.Contains("tratamiento", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Normalizar_RecortaElMotivo()
    {
        var consulta = UnaConsultaValida();
        consulta.Motivo = "  Chequeo general  ";

        consulta.Normalizar();

        Assert.Equal("Chequeo general", consulta.Motivo);
    }

    [Fact]
    public void Normalizar_ConDiagnosticoVacioLoDejaComoNull()
    {
        var consulta = UnaConsultaValida();
        consulta.Diagnostico = "   ";

        consulta.Normalizar();

        Assert.Null(consulta.Diagnostico);
    }

    private static Consulta UnaConsultaValida()
    {
        return new Consulta
        {
            MascotaId = Guid.NewGuid(),
            Fecha = DateTime.Today,
            Motivo = "Chequeo anual"
        };
    }
}
