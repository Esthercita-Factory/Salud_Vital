using System.Globalization;
using Microsoft.AspNetCore.Localization;
using SaludVital.Infra;
using SaludVital.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(opciones =>
{
    opciones.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
});

builder.Services.AddRequestLocalization(opciones =>
{
    var esCo = new CultureInfo("es-CO");
    opciones.DefaultRequestCulture = new RequestCulture(esCo);
    opciones.SupportedCultures = [esCo];
    opciones.SupportedUICultures = [esCo];
});

builder.Services.AddSingleton<IRepositorioMascotas, RepositorioMascotasEnMemoria>();
builder.Services.AddSingleton<IRepositorioConsultas, RepositorioConsultasEnMemoria>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRequestLocalization();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapGet("/salud", () => "Sana");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Mascotas}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

/// <summary>Punto de entrada expuesto para las pruebas de integración.</summary>
public partial class Program
{
}
