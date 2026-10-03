using System.ComponentModel.DataAnnotations;

namespace VetCare.Web.Models;

public class CrearTratamientoViewModel
{
    [Range(1, int.MaxValue,
        ErrorMessage = "Ingresa un código de consulta válido.")]
    [Display(Name = "Código de consulta")]
    public int ConsultaId { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(500)]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "Las indicaciones son obligatorias.")]
    [StringLength(2000)]
    public string Indicaciones { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    [Display(Name = "Fecha de inicio")]
    public DateTime? FechaInicio { get; set; }

    [Display(Name = "Fecha de finalización (opcional)")]
    public DateTime? FechaFin { get; set; }
}