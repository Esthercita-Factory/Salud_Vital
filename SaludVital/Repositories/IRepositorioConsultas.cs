using SaludVital.Models;

namespace SaludVital.Repositories;

public interface IRepositorioConsultas
{
    /// <summary>Devuelve las consultas de una mascota, ordenadas de la más reciente a la más antigua.</summary>
    List<Consulta> ObtenerPorMascota(Guid mascotaId);

    Consulta? BuscarPorId(Guid id);

    void Registrar(Consulta consulta);

    void Actualizar(Consulta consulta);

    bool Eliminar(Guid id);
}
