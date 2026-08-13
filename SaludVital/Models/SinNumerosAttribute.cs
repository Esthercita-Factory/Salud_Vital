using System.ComponentModel.DataAnnotations;

namespace SaludVital.Models;

/// <summary>Valida que un texto no contenga dígitos.</summary>
public sealed class SinNumerosAttribute : ValidationAttribute
{
    public SinNumerosAttribute()
    {
        ErrorMessage = "No puede contener números.";
    }

    public override bool IsValid(object? valor)
    {
        if (valor is not string texto || string.IsNullOrWhiteSpace(texto))
        {
            return true;
        }

        return !texto.Any(char.IsDigit);
    }
}
