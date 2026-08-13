using Microsoft.AspNetCore.Mvc;
using SaludVital.Models;
using SaludVital.Repositories;

namespace SaludVital.Controllers;

public class MascotasController : Controller
{
    private readonly IRepositorioMascotas _repositorioMascotas;

    public MascotasController(IRepositorioMascotas repositorioMascotas)
    {
        _repositorioMascotas = repositorioMascotas;
    }

    public IActionResult Index(string? busqueda)
    {
        var mascotas = _repositorioMascotas.ObtenerTodas(busqueda);
        ViewBag.Busqueda = busqueda;
        return View(mascotas);
    }

    public IActionResult Detalles(Guid id)
    {
        var mascota = _repositorioMascotas.BuscarPorId(id);
        if (mascota is null)
        {
            return NotFound();
        }

        return View(mascota);
    }

    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(Mascota mascota)
    {
        if (!ModelState.IsValid)
        {
            return View(mascota);
        }

        mascota.Id = Guid.NewGuid();
        mascota.Normalizar();

        _repositorioMascotas.Registrar(mascota);

        TempData["Mensaje"] = $"Se registró a {mascota.Nombre} correctamente.";
        return RedirectToAction(nameof(Detalles), new { id = mascota.Id });
    }

    public IActionResult Editar(Guid id)
    {
        var mascota = _repositorioMascotas.BuscarPorId(id);
        if (mascota is null)
        {
            return NotFound();
        }

        return View(mascota);
    }

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
            return View(mascota);
        }

        mascota.Normalizar();
        _repositorioMascotas.Actualizar(mascota);

        TempData["Mensaje"] = $"Se actualizó a {mascota.Nombre} correctamente.";
        return RedirectToAction(nameof(Detalles), new { id = mascota.Id });
    }

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
}
