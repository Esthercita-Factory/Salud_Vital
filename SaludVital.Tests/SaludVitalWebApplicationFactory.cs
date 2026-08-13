using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SaludVital.Repositories;

namespace SaludVital.Tests;

public class SaludVitalWebApplicationFactory : WebApplicationFactory<Program>
{
    /// <summary>Reemplaza los repositorios por otros nuevos, para que cada prueba empiece limpia.</summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureTestServices(servicios =>
        {
            var mascotas = new RepositorioMascotasEnMemoria();
            servicios.RemoveAll(typeof(IRepositorioMascotas));
            servicios.AddSingleton<IRepositorioMascotas>(mascotas);
            servicios.RemoveAll(typeof(IRepositorioConsultas));
            servicios.AddSingleton<IRepositorioConsultas>(new RepositorioConsultasEnMemoria(mascotas));
        });
    }
}
