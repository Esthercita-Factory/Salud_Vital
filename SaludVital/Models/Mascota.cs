using System.ComponentModel.DataAnnotations;

namespace SaludVital.Models;

public enum Sexo
{
    Macho,
    Hembra
}

public class Mascota
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre {2} y {1} caracteres.")]
    [SinNumeros]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La especie es obligatoria.")]
    [SinNumeros]
    [Display(Name = "Especie")]
    public string Especie { get; set; } = string.Empty;

    [Required(ErrorMessage = "La raza es obligatoria.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "La raza debe tener entre {2} y {1} caracteres.")]
    [SinNumeros]
    [Display(Name = "Raza")]
    public string Raza { get; set; } = string.Empty;

    [Display(Name = "Sexo")]
    public Sexo Sexo { get; set; }

    [Range(0, 600, ErrorMessage = "La edad debe estar entre {1} y {2} meses.")]
    [Display(Name = "Edad en meses")]
    public int EdadEnMeses { get; set; }

    [Range(0.1, 300, ErrorMessage = "El peso debe estar entre {1} y {2} kg.")]
    [Display(Name = "Peso (kg)")]
    public decimal PesoEnKg { get; set; }

    [Required(ErrorMessage = "El nombre del dueño es obligatorio.")]
    [StringLength(80, ErrorMessage = "El nombre del dueño no puede pasar de {1} caracteres.")]
    [SinNumeros]
    [Display(Name = "Dueño")]
    public string NombreDelDuenio { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(@"^[0-9+\-() ]{7,20}$", ErrorMessage = "El teléfono no tiene un formato válido.")]
    [Display(Name = "Teléfono del dueño")]
    public string TelefonoDelDuenio { get; set; } = string.Empty;

    [Display(Name = "Activo")]
    public bool EstaActivo { get; set; } = true;

    [StringLength(500, ErrorMessage = "Las notas no pueden pasar de {1} caracteres.")]
    [Display(Name = "Notas")]
    public string? Notas { get; set; }

    public List<Consulta> Consultas { get; set; } = [];

    /// <summary>Normaliza el texto para guardarlo de forma consistente.</summary>
    public void Normalizar()
    {
        Nombre = Nombre.Trim();
        Especie = Especie.Trim();
        Raza = Raza.Trim();
        NombreDelDuenio = NombreDelDuenio.Trim();
        TelefonoDelDuenio = TelefonoDelDuenio.Trim();
        Notas = string.IsNullOrWhiteSpace(Notas) ? null : Notas.Trim();
    }
}
