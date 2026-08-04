using SaludVital.Models;
using SaludVital.Repositories;

namespace SaludVital.UI;

public class ManagerMascota
{
    public static void CrearUnaMascota()
    {
        try
        {
            Console.Write("por favor ingrese el nombre de la mascota: ");
            string? nombre = Console.ReadLine();

            Console.Write("por favor ingrese la raza de la mascota: ");
            string? raza = Console.ReadLine();

            Console.Write("por favor ingrese la edad en meses de la mascota ");
            int edadEnMeses = Convert.ToInt32(Console.ReadLine() ?? "0");

            var mascotaNueva = new Mascota(nombre, raza, edadEnMeses);

            MascotaRepository.RegistrarMascota(mascotaNueva);
            Console.WriteLine("Mascota registrada con exito");
        }
        catch (FormatException)
        {
            Console.WriteLine("La edad debe ser un número válido");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Dato inválido: {ex.Message}");
        }
        catch (Exception)
        {
            Console.WriteLine("Ocurrió un error inesperado.");
        }
    }



//editar 
    public static void EditarMascota()
    {
        try
        {
            Console.Write("Ingrese el id de la mascota: ");
            if (!Guid.TryParse(Console.ReadLine(), out Guid id))
            {
                Console.WriteLine("El id ingresado no es válido.");
                return;
            }

            var mascota = MascotaRepository.BuscarPorId(id);

            if (mascota is null)
            {
                Console.WriteLine("No se encontró la mascota.");
                return;
            }

            Console.Write("Nuevo nombre: ");
            mascota.Nombre = Console.ReadLine().Trim().ToLower();

            Console.Write("Nueva raza: ");
            mascota.Raza = Console.ReadLine().Trim().ToLower();

            Console.Write("Nueva edad en meses: ");
            mascota.EdadEnMeses = int.Parse(Console.ReadLine() ?? "0");

            Console.WriteLine("Mascota actualizada correctamente.");
        }
        catch (FormatException)
        {
            Console.WriteLine("El ID debe contener varios caracteres");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Dato no valido: {ex.Message}");
        }
    }

    public static void EliminarMascota()
    {
        Console.Write("Ingrese el id de la mascota: ");
        if (!Guid.TryParse(Console.ReadLine(), out Guid id))
        {
            Console.WriteLine("El id ingresado no es válido.");
            return;
        }

        var eliminada = MascotaRepository.EliminarPorId(id);
        if (eliminada)
        {
            Console.WriteLine("Mascota eliminada correctamente.");
        }
        else
        {
            Console.WriteLine("No se encontró la mascota.");
        }
    }


public static void MostrarTodasLasMascotas()
        {
            var mascotasDeLaBaseDeDatos = MascotaRepository.ListMascotas();

            foreach (var mascota in mascotasDeLaBaseDeDatos)
            {
                mascota.MostrarDetalles();
                Console.WriteLine("--------------");
            }
        }
    }

