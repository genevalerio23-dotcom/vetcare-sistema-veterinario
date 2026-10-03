using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VetCare.Web.Models;

public class CrearConsultaViewModel
{
    [Range(1, int.MaxValue,
        ErrorMessage = "Selecciona una mascota.")]
    [Display(Name = "Mascota")]
    public int MascotaId { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "Selecciona un veterinario.")]
    [Display(Name = "Veterinario")]
    public int VeterinarioId { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "El código de cita debe ser mayor que cero.")]
    [Display(Name = "Código de cita (opcional)")]
    public int? CitaId { get; set; }

    [Required(ErrorMessage = "La fecha y hora son obligatorias.")]
    [Display(Name = "Fecha y hora de atención")]
    public DateTime? Fecha { get; set; }

    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [StringLength(250)]
    public string Motivo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El diagnóstico es obligatorio.")]
    [StringLength(2000)]
    [Display(Name = "Diagnóstico")]
    public string Diagnostico { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Observaciones { get; set; }

    [BindNever]
    public List<SelectListItem> Mascotas { get; set; } = new();

    [BindNever]
    public List<SelectListItem> Veterinarios { get; set; } = new();
}