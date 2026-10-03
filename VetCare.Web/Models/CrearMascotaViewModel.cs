using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VetCare.Web.Models;

public class CrearMascotaViewModel
{
    [Range(1, int.MaxValue,
        ErrorMessage = "Selecciona un propietario.")]
    [Display(Name = "Propietario")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La especie es obligatoria.")]
    [StringLength(50)]
    public string Especie { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Raza { get; set; }

    [Required(ErrorMessage = "Selecciona el sexo.")]
    [RegularExpression("^(Macho|Hembra)$",
        ErrorMessage = "Selecciona Macho o Hembra.")]
    public string Sexo { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999.99",
        ErrorMessage = "El peso debe estar entre 0.01 y 9999.99 kg.")]
    [Display(Name = "Peso (kg)")]
    public decimal Peso { get; set; }

    [BindNever]
    public List<SelectListItem> Propietarios { get; set; } = new();
}