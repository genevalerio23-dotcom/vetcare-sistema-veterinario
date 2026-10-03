using System.Collections.Generic;

namespace VetCare.Application.DTOs;

public class HistorialMascotaDto
{
    public int MascotaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string Raza { get; set; } = string.Empty;
    public string Sexo { get; set; } = string.Empty;
    public decimal Peso { get; set; }
    public string Propietario { get; set; } = string.Empty;
    public List<ConsultaHistorialDto> Consultas { get; set; } = new();
}