using System;
using VetCare.Domain.Exceptions;

namespace VetCare.Domain.Entities;

public class Tratamiento
{
    public int Id { get; private set; }
    public int ConsultaId { get; private set; }
    public string Descripcion { get; private set; } = string.Empty;
    public string Indicaciones { get; private set; } = string.Empty;
    public DateTime FechaInicio { get; private set; }
    public DateTime? FechaFin { get; private set; }

    private Tratamiento()
    {
    }

    public Tratamiento(
        int consultaId,
        string descripcion,
        string indicaciones,
        DateTime fechaInicio,
        DateTime? fechaFin)
    {
        if (consultaId <= 0)
        {
            throw new ReglaNegocioException(
                "Debe seleccionar una consulta válida.");
        }

        var descripcionLimpia = (descripcion ?? string.Empty).Trim();
        var indicacionesLimpias = (indicaciones ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(descripcionLimpia) ||
            descripcionLimpia.Length > 500)
        {
            throw new ReglaNegocioException(
                "La descripción es obligatoria y no debe superar 500 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(indicacionesLimpias) ||
            indicacionesLimpias.Length > 2000)
        {
            throw new ReglaNegocioException(
                "Las indicaciones son obligatorias y no deben superar 2000 caracteres.");
        }

        if (fechaInicio == default)
        {
            throw new ReglaNegocioException(
                "La fecha de inicio no es válida.");
        }

        if (fechaFin.HasValue && fechaFin.Value.Date < fechaInicio.Date)
        {
            throw new ReglaNegocioException(
                "La fecha final no puede ser anterior a la fecha de inicio.");
        }

        ConsultaId = consultaId;
        Descripcion = descripcionLimpia;
        Indicaciones = indicacionesLimpias;
        FechaInicio = fechaInicio.Date;
        FechaFin = fechaFin?.Date;
    }
}