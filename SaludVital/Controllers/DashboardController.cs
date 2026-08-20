using Microsoft.AspNetCore.Mvc;
using SaludVital.Models;
using SaludVital.Repositories;

namespace SaludVital.Controllers;

public class DashboardController : Controller
{
    private readonly IRepositorioMascotas _repositorioMascotas;

    public DashboardController(IRepositorioMascotas repositorioMascotas)
    {
        _repositorioMascotas = repositorioMascotas;
    }

    public IActionResult Index()
    {
        var mascotas = _repositorioMascotas.ObtenerTodas();
        var todasConsultas = mascotas
            .SelectMany(m => m.Consultas.Select(c => new ConsultaDashboardItem { Consulta = c, Mascota = m }))
            .OrderByDescending(item => item.Consulta.Fecha)
            .ToList();

        ViewBag.TotalMascotas = mascotas.Count;
        ViewBag.MascotasActivas = mascotas.Count(m => m.EstaActivo);
        ViewBag.MascotasInactivas = mascotas.Count(m => !m.EstaActivo);
        ViewBag.TotalConsultas = todasConsultas.Count;
        ViewBag.ConsultasRecientes = todasConsultas.Take(5).ToList();
        ViewBag.HayMasConsultas = todasConsultas.Count > 5;
        ViewBag.PorEspecie = mascotas
            .GroupBy(m => m.Especie)
            .ToDictionary(g => g.Key, g => g.Count());

        return View();
    }
}
