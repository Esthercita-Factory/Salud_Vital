using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SaludVital.Infra;

/// <summary>
/// Enlaza los <see cref="decimal"/> aceptando tanto el separador decimal
/// invariante (punto) como el de la cultura activa (coma en es-CO).
/// </summary>
public sealed class DecimalModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext contexto)
    {
        var tipo = contexto.Metadata.ModelType;
        if (tipo == typeof(decimal) || tipo == typeof(decimal?))
        {
            return new DecimalModelBinder();
        }

        return null;
    }
}

public sealed class DecimalModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext contexto)
    {
        var nombreDelValor = contexto.ModelName;
        var valor = contexto.ValueProvider.GetValue(nombreDelValor).FirstValue;

        if (string.IsNullOrWhiteSpace(valor))
        {
            return Task.CompletedTask;
        }

        var valorTrimeado = valor.Trim();
        var estilos = NumberStyles.Number;

        // Se elige la cultura según el separador decimal que traiga el texto:
        // con coma se interpreta como es-CO (10,5 -> 10,5) y con punto como
        // invariante (10.5 -> 10,5). Si trae ambos, se usa la cultura activa.
        var tieneComa = valorTrimeado.Contains(',');
        var tienePunto = valorTrimeado.Contains('.');

        var cultura = tieneComa && !tienePunto
            ? CultureInfo.CurrentCulture
            : CultureInfo.InvariantCulture;

        if (decimal.TryParse(valorTrimeado, estilos, cultura, out var resultado))
        {
            contexto.Result = ModelBindingResult.Success(resultado);
            return Task.CompletedTask;
        }

        contexto.ModelState.AddModelError(nombreDelValor, "El valor debe ser un número válido.");
        return Task.CompletedTask;
    }
}
