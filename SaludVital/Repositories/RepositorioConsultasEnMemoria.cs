using SaludVital.Models;

namespace SaludVital.Repositories;

public class RepositorioConsultasEnMemoria : IRepositorioConsultas
{
    private readonly IRepositorioMascotas _repositorioMascotas;

    public RepositorioConsultasEnMemoria(IRepositorioMascotas repositorioMascotas)
    {
        _repositorioMascotas = repositorioMascotas;
    }

    public List<Consulta> ObtenerPorMascota(Guid mascotaId)
    {
        var mascota = _repositorioMascotas.BuscarPorId(mascotaId);
        return mascota is null
            ? []
            : [.. mascota.Consultas.OrderByDescending(c => c.Fecha)];
    }

    public Consulta? BuscarPorId(Guid id)
    {
        return TodasLasConsultas().FirstOrDefault(c => c.Id == id);
    }

    public void Registrar(Consulta consulta)
    {
        var mascota = _repositorioMascotas.BuscarPorId(consulta.MascotaId);
        if (mascota is null)
        {
            throw new InvalidOperationException($"No existe la mascota con id {consulta.MascotaId}.");
        }

        mascota.Consultas.Add(consulta);
    }

    public void Actualizar(Consulta consulta)
    {
        var existente = BuscarPorId(consulta.Id);
        if (existente is null)
        {
            throw new InvalidOperationException($"No existe la consulta con id {consulta.Id}.");
        }

        existente.Fecha = consulta.Fecha;
        existente.Estado = consulta.Estado;
        existente.Motivo = consulta.Motivo;
        existente.Diagnostico = consulta.Diagnostico;
        existente.Tratamiento = consulta.Tratamiento;
    }

    public bool Eliminar(Guid id)
    {
        var consulta = BuscarPorId(id);
        if (consulta is null)
        {
            return false;
        }

        var mascota = _repositorioMascotas.BuscarPorId(consulta.MascotaId);
        return mascota is not null && mascota.Consultas.Remove(consulta);
    }

    private IEnumerable<Consulta> TodasLasConsultas()
    {
        return _repositorioMascotas
            .ObtenerTodas()
            .SelectMany(m => m.Consultas);
    }
}
