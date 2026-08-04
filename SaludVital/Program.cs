using SaludVital.UI;

string? opcion;
do
{
    ManagerUser.MostraMenu();

    opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            ManagerMascota.CrearUnaMascota();
            break;
        case "2":
            ManagerMascota.MostrarTodasLasMascotas();
            break;
        case "3":
            ManagerMascota.EditarMascota();
            break;
        case "4":
            ManagerMascota.EliminarMascota();
            break;
        case "0":
            Console.WriteLine("Have a good day!");
            break;
        default:
            Console.WriteLine("te equivocaste de opcion");
            break;
    }
} while (opcion != "0");
