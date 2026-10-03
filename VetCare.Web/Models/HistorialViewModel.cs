using Microsoft.AspNetCore.Mvc.Rendering;
using VetCare.Application.DTOs;

namespace VetCare.Web.Models;

public class HistorialViewModel
{
    public int? MascotaId { get; set; }

    public List<SelectListItem> Mascotas { get; set; } = new();

    public HistorialMascotaDto? Historial { get; set; }
}