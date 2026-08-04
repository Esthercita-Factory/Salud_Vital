namespace SaludVital.Models;

public class Mascota
{
    public Guid Id { get; set; }
    public string Nombre { get; set; }
    public string Raza { get; set; }
    public int EdadEnMeses { get; set; }

    public Mascota(string nombre, string raza, int edadEnMeses)
    {
        Id = Guid.NewGuid();
        Nombre = nombre.Trim().ToLower();
        Raza = raza.Trim().ToLower();
        EdadEnMeses = edadEnMeses;
    }

    public void MostrarDetalles()
    {
        Console.WriteLine($"Id: {Id}");
        Console.WriteLine($"Nombre: {Nombre}");
        Console.WriteLine($"Raza: {Raza}");
        Console.WriteLine($"Edad en Meses: {EdadEnMeses}");
    }
}
