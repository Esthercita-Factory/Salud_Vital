using SaludVital.Models;

namespace SaludVital.Repositories;

public interface IRepositorioMascotas
{
    /// <summary>Devuelve todas las mascotas, filtradas por el texto de búsqueda si se pasa.</summary>
    List<Mascota> ObtenerTodas(string? busqueda = null);

    Mascota? BuscarPorId(Guid id);

    void Registrar(Mascota mascota);

    void Actualizar(Mascota mascota);

    bool Eliminar(Guid id);
}
