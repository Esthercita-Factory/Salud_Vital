using SaludVital.Models;

namespace SaludVital.Repositories;

public static class MascotaRepository
{
    public static List<Mascota> Mascotas { get; set; }

    static MascotaRepository()
    {
        Mascotas =
        [
            new Mascota("Firulais", "Criollo", 36),
            new Mascota("Luna", "Labrador Retriever", 18),
            new Mascota("Rocky", "Bulldog Frances", 42),
            new Mascota("Michi", "Siames", 24),
            new Mascota("Toby", "Beagle", 60),
            new Mascota("Nala", "Golden Retriever", 12),
            new Mascota("Simba", "Persa", 30),
            new Mascota("Max", "Pastor Aleman", 54),
            new Mascota("Kira", "Husky Siberiano", 27),
            new Mascota("Pelusa", "Angora", 9),
            new Mascota("Bruno", "Rottweiler", 48),
            new Mascota("Canela", "Cocker Spaniel", 21),
            new Mascota("Coco", "Chihuahua", 15),
            new Mascota("Sasha", "Border Collie", 33),
            new Mascota("Manchas", "Dalmata", 39),
            new Mascota("Nube", "Bichon Maltes", 6),
            new Mascota("Zeus", "Gran Danes", 45),
            new Mascota("Mia", "Bengali", 11),
            new Mascota("Duque", "Schnauzer", 66),
            new Mascota("Pepa", "Salchicha", 29)
        ];
    }

    // CREATE
    public static void RegistrarMascota(Mascota mascotaNueva)
    {
        Mascotas.Add(mascotaNueva);
    }

    // READ
    public static List<Mascota> ListMascotas()
    {
        return Mascotas;
    }
    

    public static Mascota? BuscarPorId(Guid id)
    {
        return Mascotas.FirstOrDefault(mascota => mascota.Id
                                                  == id);   
    }

    public static bool EliminarPorId(Guid id)
    {
        var mascota = BuscarPorId(id);

        if (mascota is null)
        {
            return false;
        }

        return Mascotas.Remove(mascota);
    }
}                                           
             