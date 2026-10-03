using System;
using System.Collections.Generic;

namespace VetCare.Application.DTOs;

public class ConsultaHistorialDto
{
    public int Id { get; set; }
    public int? CitaId { get; set; }
    public DateTime Fecha { get; set; }
    public string Veterinario { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public string Diagnostico { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;
    public List<TratamientoDto> Tratamientos { get; set; } = new();
}