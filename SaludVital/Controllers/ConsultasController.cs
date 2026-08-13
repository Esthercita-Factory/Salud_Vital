using Microsoft.AspNetCore.Mvc;
using SaludVital.Models;
using SaludVital.Repositories;

namespace SaludVital.Controllers;

public class ConsultasController : Controller
{
    private readonly IRepositorioConsultas _repositorioConsultas;
    private readonly IRepositorioMascotas _repositorioMascotas;

    public ConsultasController(IRepositorioConsultas repositorioConsultas, IRepositorioMascotas repositorioMascotas)
    {
        _repositorioConsultas = repositorioConsultas;
        _repositorioMascotas = repositorioMascotas;
    }

    public IActionResult Crear(Guid mascotaId)
    {
        var mascota = _repositorioMascotas.BuscarPorId(mascotaId);
        if (mascota is null)
        {
            return NotFound();
        }

        ViewBag.Mascota = mascota;
        return View(new Consulta { MascotaId = mascotaId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(Consulta consulta)
    {
        if (!ModelState.IsValid)
        {
            var mascotaParaFormulario = _repositorioMascotas.BuscarPorId(consulta.MascotaId);
            if (mascotaParaFormulario is null)
            {
                return NotFound();
            }

            ViewBag.Mascota = mascotaParaFormulario;
            return View(consulta);
        }

        consulta.Id = Guid.NewGuid();
        consulta.Normalizar();

        _repositorioConsultas.Registrar(consulta);

        TempData["Mensaje"] = "La consulta se registró correctamente.";
        return RedirectToAction(nameof(MascotasController.Detalles), "Mascotas", new { id = consulta.MascotaId });
    }

    public IActionResult Editar(Guid id)
    {
        var consulta = _repositorioConsultas.BuscarPorId(id);
        if (consulta is null)
        {
            return NotFound();
        }

        ViewBag.Mascota = _repositorioMascotas.BuscarPorId(consulta.MascotaId);
        return View(consulta);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(Guid id, Consulta consulta)
    {
        if (id != consulta.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Mascota = _repositorioMascotas.BuscarPorId(consulta.MascotaId);
            return View(consulta);
        }

        consulta.Normalizar();
        _repositorioConsultas.Actualizar(consulta);

        TempData["Mensaje"] = "La consulta se actualizó correctamente.";
        return RedirectToAction(nameof(MascotasController.Detalles), "Mascotas", new { id = consulta.MascotaId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ConfirmarEliminar(Guid id)
    {
        var consulta = _repositorioConsultas.BuscarPorId(id);
        if (consulta is null)
        {
            return NotFound();
        }

        var mascotaId = consulta.MascotaId;
        _repositorioConsultas.Eliminar(id);

        TempData["Mensaje"] = "La consulta se eliminó correctamente.";
        return RedirectToAction(nameof(MascotasController.Detalles), "Mascotas", new { id = mascotaId });
    }
}
