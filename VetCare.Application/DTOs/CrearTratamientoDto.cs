using System;

namespace VetCare.Application.DTOs;

public class CrearTratamientoDto
{
    public int ConsultaId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Indicaciones { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}