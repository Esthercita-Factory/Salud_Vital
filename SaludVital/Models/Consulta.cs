using System.ComponentModel.DataAnnotations;

namespace SaludVital.Models;

public class Consulta
{
    public Guid Id { get; set; }

    public Guid MascotaId { get; set; }

    public Mascota? Mascota { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha")]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "El motivo debe tener entre {2} y {1} caracteres.")]
    [Display(Name = "Motivo")]
    public string Motivo { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "El diagnóstico no puede pasar de {1} caracteres.")]
    [Display(Name = "Diagnóstico")]
    public string? Diagnostico { get; set; }

    [StringLength(500, ErrorMessage = "El tratamiento no puede pasar de {1} caracteres.")]
    [Display(Name = "Tratamiento")]
    public string? Tratamiento { get; set; }

    /// <summary>Normaliza el texto para guardarlo de forma consistente.</summary>
    public void Normalizar()
    {
        Motivo = Motivo.Trim();
        Diagnostico = string.IsNullOrWhiteSpace(Diagnostico) ? null : Diagnostico.Trim();
        Tratamiento = string.IsNullOrWhiteSpace(Tratamiento) ? null : Tratamiento.Trim();
    }
}
