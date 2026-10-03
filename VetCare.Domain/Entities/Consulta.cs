using System;
using VetCare.Domain.Exceptions;

namespace VetCare.Domain.Entities;

public class Consulta
{
    public int Id { get; private set; }
    public int MascotaId { get; private set; }
    public int VeterinarioId { get; private set; }
    public int? CitaId { get; private set; }
    public DateTime Fecha { get; private set; }
    public string Motivo { get; private set; } = string.Empty;
    public string Diagnostico { get; private set; } = string.Empty;
    public string Observaciones { get; private set; } = string.Empty;

    private Consulta()
    {
    }

    public Consulta(
        int mascotaId,
        int veterinarioId,
        int? citaId,
        DateTime fecha,
        string motivo,
        string diagnostico,
        string observaciones)
    {
        if (mascotaId <= 0)
        {
            throw new ReglaNegocioException(
                "Debe seleccionar una mascota válida.");
        }

        if (veterinarioId <= 0)
        {
            throw new ReglaNegocioException(
                "Debe seleccionar un veterinario válido.");
        }

        if (citaId.HasValue && citaId.Value <= 0)
        {
            throw new ReglaNegocioException(
                "La cita seleccionada no es válida.");
        }

        if (fecha == default)
        {
            throw new ReglaNegocioException(
                "La fecha de la consulta no es válida.");
        }

        var motivoLimpio = (motivo ?? string.Empty).Trim();
        var diagnosticoLimpio = (diagnostico ?? string.Empty).Trim();
        var observacionesLimpias = (observaciones ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(motivoLimpio) ||
            motivoLimpio.Length > 250)
        {
            throw new ReglaNegocioException(
                "El motivo es obligatorio y no debe superar 250 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(diagnosticoLimpio) ||
            diagnosticoLimpio.Length > 2000)
        {
            throw new ReglaNegocioException(
                "El diagnóstico es obligatorio y no debe superar 2000 caracteres.");
        }

        if (observacionesLimpias.Length > 2000)
        {
            throw new ReglaNegocioException(
                "Las observaciones no deben superar 2000 caracteres.");
        }

        MascotaId = mascotaId;
        VeterinarioId = veterinarioId;
        CitaId = citaId;
        Fecha = fecha;
        Motivo = motivoLimpio;
        Diagnostico = diagnosticoLimpio;
        Observaciones = observacionesLimpias;
    }
}