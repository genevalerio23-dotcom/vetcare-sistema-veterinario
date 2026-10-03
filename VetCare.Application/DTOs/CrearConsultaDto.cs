using System;

namespace VetCare.Application.DTOs;

public class CrearConsultaDto
{
    public int MascotaId { get; set; }
    public int VeterinarioId { get; set; }
    public int? CitaId { get; set; }
    public DateTime Fecha { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string Diagnostico { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;
}