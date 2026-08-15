using SaludVital.Models;

namespace SaludVital.Repositories;

public class RepositorioMascotasEnMemoria : IRepositorioMascotas
{
    private readonly List<Mascota> _mascotas;

    public RepositorioMascotasEnMemoria()
    {
        _mascotas = CargarDatosDeEjemplo();
    }

    public List<Mascota> ObtenerTodas(string? busqueda = null)
    {
        if (string.IsNullOrWhiteSpace(busqueda))
        {
            return [.. _mascotas];
        }

        var termino = busqueda.Trim().ToLowerInvariant();

        return _mascotas
            .Where(m => m.Nombre.Contains(termino, StringComparison.OrdinalIgnoreCase)
                        || m.Especie.Contains(termino, StringComparison.OrdinalIgnoreCase)
                        || m.Raza.Contains(termino, StringComparison.OrdinalIgnoreCase)
                        || m.NombreDelDuenio.Contains(termino, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public Mascota? BuscarPorId(Guid id)
    {
        return _mascotas.FirstOrDefault(m => m.Id == id);
    }

    public void Registrar(Mascota mascota)
    {
        _mascotas.Add(mascota);
    }

    public void Actualizar(Mascota mascota)
    {
        var existente = BuscarPorId(mascota.Id);
        if (existente is null)
        {
            throw new InvalidOperationException($"No existe la mascota con id {mascota.Id}.");
        }

        existente.Nombre = mascota.Nombre;
        existente.Especie = mascota.Especie;
        existente.Raza = mascota.Raza;
        existente.Sexo = mascota.Sexo;
        existente.EdadEnMeses = mascota.EdadEnMeses;
        existente.PesoEnKg = mascota.PesoEnKg;
        existente.NombreDelDuenio = mascota.NombreDelDuenio;
        existente.TelefonoDelDuenio = mascota.TelefonoDelDuenio;
        existente.EstaActivo = mascota.EstaActivo;
        existente.Notas = mascota.Notas;
    }

    public bool Eliminar(Guid id)
    {
        var mascota = BuscarPorId(id);
        return mascota is not null && _mascotas.Remove(mascota);
    }

    private static List<Mascota> CargarDatosDeEjemplo()
    {
        return
        [
            NuevaMascota("Firulais", "Perro", "Criollo", Sexo.Macho, 36, 18.5m, "Carlos Gómez", "3001234567", "Vacunas al día"),
            NuevaMascota("Luna", "Perro", "Labrador", Sexo.Hembra, 18, 24.0m, "María Pérez", "3107654321", "En control de peso"),
            NuevaMascota("Rocky", "Conejo", "Mini Lop", Sexo.Macho, 14, 1.9m, "Jorge Ruiz", "3209876543", "Le encanta la zanahoria"),
            NuevaMascota("Michi", "Gato", "Siamés", Sexo.Hembra, 24, 4.5m, "Laura Torres", "3012345678", "Esterilizada"),
            NuevaMascota("Toby", "Conejo", "Holandés", Sexo.Macho, 20, 2.3m, "Andrés Díaz", "3112233445", "Necesita espacio para saltar"),
            NuevaMascota("Nala", "Loro", "Cacatúa ninfa", Sexo.Hembra, 16, 0.35m, "Sofía Castro", "3154455667", "Repite sonidos y silbidos"),
            NuevaMascota("Simba", "Gato", "Persa", Sexo.Macho, 30, 5.1m, "Ricardo Mora", "3129988776", "Cepillado frecuente"),
            NuevaMascota("Max", "Perro", "Pastor Alemán", Sexo.Macho, 54, 34.5m, "Diana Herrera", "3162211998", "Entrenamiento en curso"),
            NuevaMascota("Kira", "Loro", "Cotorra argentina", Sexo.Hembra, 30, 0.28m, "Fernando Vega", "3187766554", "Pico muy curioso"),
            NuevaMascota("Pelusa", "Gato", "Angora", Sexo.Hembra, 9, 3.2m, "Paula Ríos", "3134455887", "Alimentación especial")
        ];
    }

    private static Mascota NuevaMascota(
        string nombre,
        string especie,
        string raza,
        Sexo sexo,
        int edadEnMeses,
        decimal pesoEnKg,
        string duenio,
        string telefono,
        string? notas = null)
    {
        return new Mascota
        {
            Id = Guid.NewGuid(),
            Nombre = nombre,
            Especie = especie,
            Raza = raza,
            Sexo = sexo,
            EdadEnMeses = edadEnMeses,
            PesoEnKg = pesoEnKg,
            NombreDelDuenio = duenio,
            TelefonoDelDuenio = telefono,
            EstaActivo = true,
            Notas = notas
        };
    }
}
