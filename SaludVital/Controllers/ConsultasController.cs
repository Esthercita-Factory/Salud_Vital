using Microsoft.AspNetCore.Mvc;
using SaludVital.Models;
using SaludVital.Repositories;

namespace SaludVital.Controllers;

// Las consultas dependen de una mascota: toda acción termina volviendo a la ficha
// de la mascota y necesita el repositorio de mascotas además del de consultas.
public class ConsultasController : Controller
{
    private readonly IRepositorioConsultas _repositorioConsultas;
    private readonly IRepositorioMascotas _repositorioMascotas;

    public ConsultasController(IRepositorioConsultas repositorioConsultas, IRepositorioMascotas repositorioMascotas)
    {
        _repositorioConsultas = repositorioConsultas;
        _repositorioMascotas = repositorioMascotas;
    }

    // GET: muestra el formulario de una consulta nueva para una mascota concreta.
    // El id de la mascota llega por la URL (asp-route-mascotaId) y se guarda en un campo oculto.
    public IActionResult Crear(Guid mascotaId)
    {
        var mascota = _repositorioMascotas.BuscarPorId(mascotaId);
        if (mascota is null)
        {
            return NotFound();
        }

        // ViewBag: forma dinámica de pasar datos a la vista sin usar el modelo.
        ViewBag.Mascota = mascota;
        return View(new Consulta { MascotaId = mascotaId });
    }

    // POST: procesa el formulario. Si la validación falla, repinta la vista con los errores.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(Consulta consulta)
    {
        if (!ModelState.IsValid)
        {
            // Se vuelve a buscar la mascota porque el ViewBag no sobrevive a la petición.
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

        // PRG: guardar, avisar con TempData y redirigir a la ficha de la mascota.
        TempData["Mensaje"] = "La consulta se registró correctamente.";
        return RedirectToAction(nameof(MascotasController.Detalles), "Mascotas", new { id = consulta.MascotaId });
    }

    // GET: carga la consulta y su mascota para rellenar el formulario de edición.
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

    // POST: procesa la edición. Verifica que el id de la URL coincida con el del formulario.
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

    // Recibe el POST del modal de confirmación y borra la consulta.
    // Antes redirige a la ficha de la mascota (mascotaId se obtiene antes de borrar).
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
