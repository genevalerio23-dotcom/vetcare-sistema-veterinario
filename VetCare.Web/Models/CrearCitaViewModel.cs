using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VetCare.Web.Models;

public class CrearCitaViewModel
{
    [Range(1, int.MaxValue,
        ErrorMessage = "Selecciona una mascota.")]
    [Display(Name = "Mascota")]
    public int MascotaId { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "Selecciona un veterinario.")]
    [Display(Name = "Veterinario")]
    public int VeterinarioId { get; set; }

    [Required(ErrorMessage = "Selecciona la fecha y hora.")]
    [Display(Name = "Fecha y hora")]
    public DateTime? FechaHora { get; set; }

    [Required(ErrorMessage = "El motivo es obligatorio.")]
    [StringLength(250,
        ErrorMessage = "El motivo admite hasta 250 caracteres.")]
    public string Motivo { get; set; } = string.Empty;

    [BindNever]
    public List<SelectListItem> Mascotas { get; set; } = new();

    [BindNever]
    public List<SelectListItem> Veterinarios { get; set; } = new();
}