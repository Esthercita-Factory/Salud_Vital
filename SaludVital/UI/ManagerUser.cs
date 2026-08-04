namespace SaludVital.UI;

public class ManagerUser
{
    public static void MostraMenu()
    {
        Console.WriteLine("╔════════════════════════════════════════════════╗");
        Console.WriteLine("║      CLINICA VETERINARIA PATITAS FELICES       ║");
        Console.WriteLine("║         Sistema de Gestion de Mascotas         ║");
        Console.WriteLine("╠════════════════════════════════════════════════╣");
        Console.WriteLine("║                                                ║");
        Console.WriteLine("║  [1]  Registrar una nueva mascota              ║");
        Console.WriteLine("║  [2]  Listar las mascotas registradas          ║");
        Console.WriteLine("║  [3]  Editar una mascota                       ║");
        Console.WriteLine("║  [4]  Eliminar una mascota                     ║");
        Console.WriteLine("║                                                ║");
        Console.WriteLine("║                                                ║");
        Console.WriteLine("║  --------------------------------------------  ║");
        Console.WriteLine("║                                                ║");
        Console.WriteLine("║  [0]  Salir del sistema                        ║");
        Console.WriteLine("║                                                ║");
        Console.WriteLine("╚════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.Write("  >> Seleccione una opcion: ");
    }
}
