using System;
using VetCare.Domain.Exceptions;

namespace VetCare.Domain.Entities;

public class Cita
{
    public int Id { get; private set; }
    public int MascotaId { get; private set; }
    public int VeterinarioId { get; private set; }
    public DateTime FechaHora { get; private set; }
    public string Motivo { get; private set; } = string.Empty;
    public string Estado { get; private set; } = "Programada";

    public DateTime FechaHoraFin => FechaHora.AddMinutes(30);

    private Cita()
    {
    }

    public Cita(
        int mascotaId,
        int veterinarioId,
        DateTime fechaHora,
        string motivo)
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

        if (fechaHora == default ||
            fechaHora > DateTime.MaxValue.AddMinutes(-30))
        {
            throw new ReglaNegocioException(
                "La fecha y hora de la cita no son válidas.");
        }

        var motivoLimpio = (motivo ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(motivoLimpio) ||
            motivoLimpio.Length > 250)
        {
            throw new ReglaNegocioException(
                "El motivo es obligatorio y no debe superar 250 caracteres.");
        }

        MascotaId = mascotaId;
        VeterinarioId = veterinarioId;
        FechaHora = fechaHora;
        Motivo = motivoLimpio;
    }

    public void MarcarAtendida()
    {
        if (Estado != "Programada")
        {
            throw new ReglaNegocioException(
                "Solo una cita programada puede marcarse como atendida.");
        }

        Estado = "Atendida";
    }

    public void Cancelar()
    {
        if (Estado != "Programada")
        {
            throw new ReglaNegocioException(
                "Solo una cita programada puede cancelarse.");
        }

        Estado = "Cancelada";
    }
}