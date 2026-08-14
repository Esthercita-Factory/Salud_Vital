using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SaludVital.Models;

namespace SaludVital.Controllers;

// Controlador mínimo: solo maneja la página de error.
public class HomeController : Controller
{
    // La página de error no debe quedar en caché del navegador, o el usuario
    // vería siempre el mismo error aunque este ya se haya resuelto.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // RequestId: identificador único de la petición para rastrear el error.
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
