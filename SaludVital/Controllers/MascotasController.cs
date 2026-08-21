using Microsoft.AspNetCore.Mvc;
using SaludVital.Models;
using SaludVital.Repositories;

namespace SaludVital.Controllers;

// Controlador principal del CRUD de mascotas.
// Usa inyección de dependencias: el repositorio llega por el constructor y queda
// guardado en un campo de solo lectura (nunca se crea con `new`).
public class MascotasController : Controller
{
    private readonly IRepositorioMascotas _repositorioMascotas;

    public MascotasController(IRepositorioMascotas repositorioMascotas)
    {
        _repositorioMascotas = repositorioMascotas;
    }

    // GET: lista completa del catálogo; búsqueda y filtros se aplican en el encabezado.
    public IActionResult Index(string? busqueda)
    {
        var mascotas = _repositorioMascotas.ObtenerTodas();
        ViewBag.Busqueda = busqueda;
        ViewBag.TotalMascotas = mascotas.Count;
        ViewBag.MascotasActivas = mascotas.Count(m => m.EstaActivo);
        ViewBag.TotalEspecies = mascotas
            .Select(m => m.Especie)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();
        ViewBag.EspeciesDisponibles = mascotas
            .Select(m => m.Especie.Trim())
            .Where(e => e.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(e => e)
            .ToList();
        return View(mascotas);
    }

    // GET: ficha completa de una mascota. Si el id no existe, devuelve 404.
    public IActionResult Detalles(Guid id)
    {
        var mascota = _repositorioMascotas.BuscarPorId(id);
        if (mascota is null)
        {
            return NotFound();
        }

        return View(mascota);
    }

    // GET: muestra el formulario en blanco para registrar una mascota.
    public IActionResult Crear()
    {
        CargarEstadisticasPanel();
        return View();
    }

    // POST: procesa el formulario de alta.
    // ValidateAntiForgeryToken exige el token de seguridad que el formulario
    // genera con <form asp-action> (protección contra peticiones cruzadas CSRF).
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(Mascota mascota)
    {
        // Si la validación de los DataAnnotations falla, se repinta la vista
        // con los errores ya cargados en ModelState.
        if (!ModelState.IsValid)
        {
            CargarEstadisticasPanel();
            return View(mascota);
        }

        // Se asigna el id aquí en vez de en el constructor para no chocar
        // con los valores que el propio repositorio de pruebas fija.
        mascota.Id = Guid.NewGuid();
        mascota.Normalizar();

        _repositorioMascotas.Registrar(mascota);

        // PRG (Post-Redirect-Get): el mensaje se guarda en TempData y la
        // redirección evita que al recargar se reenvíe el formulario.
        TempData["Mensaje"] = $"Se registró a {mascota.Nombre} correctamente.";
        return RedirectToAction(nameof(Detalles), new { id = mascota.Id });
    }

    // GET: carga la mascota para rellenar el formulario de edición.
    public IActionResult Editar(Guid id)
    {
        var mascota = _repositorioMascotas.BuscarPorId(id);
        if (mascota is null)
        {
            return NotFound();
        }

        CargarEstadisticasPanel();
        return View(mascota);
    }

    // POST: procesa la edición. El id que llega por la URL debe coincidir
    // con el id oculto del formulario, o se devuelve 400.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(Guid id, Mascota mascota)
    {
        if (id != mascota.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            CargarEstadisticasPanel();
            return View(mascota);
        }

        mascota.Normalizar();
        _repositorioMascotas.Actualizar(mascota);

        TempData["Mensaje"] = $"Se actualizó a {mascota.Nombre} correctamente.";
        return RedirectToAction(nameof(Detalles), new { id = mascota.Id });
    }
    // Recibe el POST del modal de confirmación y borra la mascota.
    // Solo se puede disparar con POST (no con un enlace), y exige el token anti-CSRF.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ConfirmarEliminar(Guid id)
    {
        var eliminada = _repositorioMascotas.Eliminar(id);
        if (!eliminada)
        {
            return NotFound();
        }

        TempData["Mensaje"] = "La mascota se eliminó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private void CargarEstadisticasPanel()
    {
        var mascotas = _repositorioMascotas.ObtenerTodas();

        ViewBag.TotalMascotas = mascotas.Count;
        ViewBag.TotalEspecies = mascotas.Select(m => m.Especie).Distinct(StringComparer.OrdinalIgnoreCase).Count();
        ViewBag.ConsultasEnEspera = mascotas
            .SelectMany(m => m.Consultas)
            .Count(c => c.Estado == EstadoConsulta.Pendiente);
    }
}
