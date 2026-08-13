using System.ComponentModel.DataAnnotations;

namespace SaludVital.Tests;

public static class ValidadorDeModelos
{
    /// <summary>Valida un modelo con sus anotaciones de datos y devuelve los mensajes de error.</summary>
    public static List<string> ErroresDe(object modelo)
    {
        var contexto = new ValidationContext(modelo, null, null);
        var resultados = new List<ValidationResult>();

        Validator.TryValidateObject(modelo, contexto, resultados, validateAllProperties: true);

        return [.. resultados.Select(r => r.ErrorMessage ?? string.Empty)];
    }

    /// <summary>Devuelve los mensajes de error que contienen el texto indicado.</summary>
    public static List<string> ErroresQueContienen(object modelo, string texto)
    {
        return ErroresDe(modelo).Where(e => e.Contains(texto, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}
