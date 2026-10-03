using System;

namespace VetCare.Application.DTOs;

public class CrearCitaDto
{
    public int MascotaId { get; set; }
    public int VeterinarioId { get; set; }
    public DateTime FechaHora { get; set; }
    public string Motivo { get; set; } = string.Empty;
}