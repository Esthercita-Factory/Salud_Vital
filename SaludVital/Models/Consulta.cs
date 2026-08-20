using System.ComponentModel.DataAnnotations;

namespace SaludVital.Models;

public enum EstadoConsulta
{
    Pendiente,
    Completada,
    Cancelada
}

public class Consulta
{
    public Guid Id { get; set; }

    public Guid MascotaId { get; set; }

    public Mascota? Mascota { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha")]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [Display(Name = "Estado")]
    public EstadoConsulta Estado { get; set; } = EstadoConsulta.Pendiente;

    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "El motivo debe tener entre {2} y {1} caracteres.")]
    [Display(Name = "Motivo")]
    public string Motivo { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Los síntomas no pueden pasar de {1} caracteres.")]
    [Display(Name = "Síntomas")]
    public string? Sintomas { get; set; }

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
        Sintomas = string.IsNullOrWhiteSpace(Sintomas)
            ? null
            : string.Join(", ", Sintomas.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct(StringComparer.OrdinalIgnoreCase));
        Diagnostico = string.IsNullOrWhiteSpace(Diagnostico) ? null : Diagnostico.Trim();
        Tratamiento = string.IsNullOrWhiteSpace(Tratamiento) ? null : Tratamiento.Trim();
    }
}
