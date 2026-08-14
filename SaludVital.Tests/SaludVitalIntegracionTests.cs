using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace SaludVital.Tests;

public class SaludVitalIntegracionTests : IClassFixture<SaludVitalWebApplicationFactory>
{
    private readonly SaludVitalWebApplicationFactory _fabrica;

    public SaludVitalIntegracionTests(SaludVitalWebApplicationFactory fabrica)
    {
        _fabrica = fabrica;
    }

    [Fact]
    public async Task LaRaiz_DirigeDirectamenteAlCatalogoDeMascotas()
    {
        var cliente = _fabrica.CreateClient();

        var respuesta = await cliente.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var contenido = await ContenidoDecodificado(respuesta);
        Assert.Contains("Pacientes registrados en Patitas Felices", contenido);
        Assert.Contains("Firulais", contenido);
    }

    [Fact]
    public async Task ElEndpointDeSalud_RespondeSana()
    {
        var cliente = _fabrica.CreateClient();

        var respuesta = await cliente.GetAsync("/salud");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        Assert.Equal("Sana", await respuesta.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task BuscarPorNombre_FiltraElCatalogo()
    {
        var cliente = _fabrica.CreateClient();

        var respuesta = await cliente.GetAsync("/Mascotas?busqueda=firulais");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var contenido = await ContenidoDecodificado(respuesta);
        Assert.Contains("Firulais", contenido);
        Assert.DoesNotContain("Luna", contenido);
    }

    [Fact]
    public async Task ElFormularioDeRegistro_MuestraLosCamposEnEspanol()
    {
        var cliente = _fabrica.CreateClient();

        var respuesta = await cliente.GetAsync("/Mascotas/Crear");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var contenido = await ContenidoDecodificado(respuesta);
        Assert.Contains("Registrar mascota", contenido);
        Assert.Contains("Dueño", contenido);
        Assert.Contains("Edad en meses", contenido);
    }

    [Fact]
    public async Task RegistrarUnaMascota_LaAgregaAlCatalogo()
    {
        var (cliente, token) = await ClienteConToken("/Mascotas/Crear");

        var respuesta = await cliente.PostAsync("/Mascotas/Crear", FormularioDeMascota(token, new Dictionary<string, string>
        {
            ["Nombre"] = "Prueba",
            ["Especie"] = "Perro",
            ["Raza"] = "Criollo",
            ["Sexo"] = "Macho",
            ["EdadEnMeses"] = "12",
            ["PesoEnKg"] = "10.5",
            ["NombreDelDuenio"] = "Dueño de prueba",
            ["TelefonoDelDuenio"] = "3001234567"
        }));

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var contenido = await ContenidoDecodificado(respuesta);
        Assert.Contains("Ficha de Prueba", contenido);
    }

    [Fact]
    public async Task EnviarUnFormularioIncompleto_MuestraLosErroresDeValidacion()
    {
        var (cliente, token) = await ClienteConToken("/Mascotas/Crear");

        var respuesta = await cliente.PostAsync("/Mascotas/Crear",
            FormularioDeMascota(token, new Dictionary<string, string> { ["Nombre"] = "" }));

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var contenido = await ContenidoDecodificado(respuesta);
        Assert.Contains("El nombre es obligatorio.", contenido);
    }

    [Fact]
    public async Task ElPesoConPuntoDecimal_SeInterpretaBien()
    {
        var (cliente, token) = await ClienteConToken("/Mascotas/Crear");

        var respuesta = await cliente.PostAsync("/Mascotas/Crear", FormularioDeMascota(token, new Dictionary<string, string>
        {
            ["Nombre"] = "Punto",
            ["Especie"] = "Gato",
            ["Raza"] = "Criollo",
            ["Sexo"] = "Hembra",
            ["EdadEnMeses"] = "6",
            ["PesoEnKg"] = "10.5",
            ["NombreDelDuenio"] = "Dueño de prueba",
            ["TelefonoDelDuenio"] = "3001234567"
        }));

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var contenido = await ContenidoDecodificado(respuesta);
        Assert.Contains("10,5", contenido);
    }

    [Fact]
    public async Task ElPesoConComaDecimal_SeInterpretaBien()
    {
        var (cliente, token) = await ClienteConToken("/Mascotas/Crear");

        var respuesta = await cliente.PostAsync("/Mascotas/Crear", FormularioDeMascota(token, new Dictionary<string, string>
        {
            ["Nombre"] = "Coma",
            ["Especie"] = "Gato",
            ["Raza"] = "Criollo",
            ["Sexo"] = "Macho",
            ["EdadEnMeses"] = "6",
            ["PesoEnKg"] = "10,5",
            ["NombreDelDuenio"] = "Dueño de prueba",
            ["TelefonoDelDuenio"] = "3001234567"
        }));

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var contenido = await ContenidoDecodificado(respuesta);
        Assert.Contains("10,5", contenido);
    }

    [Fact]
    public async Task EnviarUnPesoNoNumerico_MuestraElErrorDelEnlazador()
    {
        var (cliente, token) = await ClienteConToken("/Mascotas/Crear");

        var respuesta = await cliente.PostAsync("/Mascotas/Crear", FormularioDeMascota(token, new Dictionary<string, string>
        {
            ["Nombre"] = "PesoMal",
            ["Especie"] = "Perro",
            ["Raza"] = "Criollo",
            ["Sexo"] = "Macho",
            ["EdadEnMeses"] = "12",
            ["PesoEnKg"] = "abc",
            ["NombreDelDuenio"] = "Dueño de prueba",
            ["TelefonoDelDuenio"] = "3001234567"
        }));

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var contenido = await ContenidoDecodificado(respuesta);
        Assert.Contains("El valor debe ser un número válido.", contenido);
    }

    [Fact]
    public async Task EliminarUnaMascota_ViaElModal_LaBorraDelCatalogo()
    {
        var (cliente, token) = await ClienteConToken("/Mascotas/Crear");

        var registro = await cliente.PostAsync("/Mascotas/Crear", FormularioDeMascota(token, new Dictionary<string, string>
        {
            ["Nombre"] = "PruebaEliminar",
            ["Especie"] = "Perro",
            ["Raza"] = "Criollo",
            ["Sexo"] = "Macho",
            ["EdadEnMeses"] = "12",
            ["PesoEnKg"] = "8.5",
            ["NombreDelDuenio"] = "Dueño de prueba",
            ["TelefonoDelDuenio"] = "3001234567"
        }));
        Assert.Equal(HttpStatusCode.OK, registro.StatusCode);

        var uriFicha = registro.RequestMessage!.RequestUri!;
        Assert.Matches("/Mascotas/Detalles/[0-9a-f-]{36}$", uriFicha.ToString());
        var id = uriFicha.Segments[^1];

        var paginaFicha = await ContenidoDecodificado(await cliente.GetAsync(uriFicha));
        var tokenFicha = Regex.Match(paginaFicha, @"name=""__RequestVerificationToken""[^>]*value=""([^""]+)""").Groups[1].Value;

        var borrado = await cliente.PostAsync($"/Mascotas/ConfirmarEliminar/{id}",
            new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("__RequestVerificationToken", tokenFicha) }));

        Assert.Equal(HttpStatusCode.OK, borrado.StatusCode);
        var contenido = await ContenidoDecodificado(borrado);
        Assert.Contains("La mascota se eliminó correctamente.", contenido);
        Assert.DoesNotContain("PruebaEliminar", contenido);
    }

    private async Task<(HttpClient Cliente, string Token)> ClienteConToken(string ruta)
    {
        var cliente = _fabrica.CreateClient();
        var pagina = await cliente.GetAsync(ruta);
        var html = await pagina.Content.ReadAsStringAsync();

        var coincidencia = Regex.Match(html, @"name=""__RequestVerificationToken""[^>]*value=""([^""]+)""");
        Assert.True(coincidencia.Success, "No se encontró el token antifalsificación en el formulario.");

        return (cliente, coincidencia.Groups[1].Value);
    }

    private static async Task<string> ContenidoDecodificado(HttpResponseMessage respuesta)
    {
        return WebUtility.HtmlDecode(await respuesta.Content.ReadAsStringAsync());
    }

    private static FormUrlEncodedContent FormularioDeMascota(string token, Dictionary<string, string> campos)
    {
        var pares = new List<KeyValuePair<string, string>>
        {
            new("__RequestVerificationToken", token),
            new("EstaActivo", "true")
        };
        pares.AddRange(campos);
        return new FormUrlEncodedContent(pares);
    }
}
