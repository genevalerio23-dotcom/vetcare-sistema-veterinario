using System;
using System.Threading.Tasks;
using VetCare.Application.DTOs;
using VetCare.Application.Interfaces;
using VetCare.Domain.Entities;
using VetCare.Domain.Exceptions;
using VetCare.Domain.Interfaces;

namespace VetCare.Application.Services;

public class CitaService : ICitaService
{
    private readonly ICitaRepository _citaRepository;
    private readonly IMascotaRepository _mascotaRepository;
    private readonly IVeterinarioRepository _veterinarioRepository;
    private readonly TimeProvider _reloj;

    public CitaService(
        ICitaRepository citaRepository,
        IMascotaRepository mascotaRepository,
        IVeterinarioRepository veterinarioRepository,
        TimeProvider reloj)
    {
        _citaRepository = citaRepository;
        _mascotaRepository = mascotaRepository;
        _veterinarioRepository = veterinarioRepository;
        _reloj = reloj;
    }

    public async Task<int> AgendarAsync(CrearCitaDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var cita = new Cita(
            dto.MascotaId,
            dto.VeterinarioId,
            dto.FechaHora,
            dto.Motivo);

        var ahora = _reloj.GetLocalNow().DateTime;

        if (cita.FechaHora <= ahora)
        {
            throw new ReglaNegocioException(
                "La cita debe programarse para una fecha y hora futuras.");
        }

        var mascota = await _mascotaRepository.ObtenerPorIdAsync(
            cita.MascotaId);

        if (mascota is null)
        {
            throw new ReglaNegocioException(
                "La mascota seleccionada no existe.");
        }

        if (!mascota.Activo)
        {
            throw new ReglaNegocioException(
                "No se pueden agendar citas para una mascota inactiva.");
        }

        var veterinario = await _veterinarioRepository.ObtenerPorIdAsync(
            cita.VeterinarioId);

        if (veterinario is null)
        {
            throw new ReglaNegocioException(
                "El veterinario seleccionado no existe.");
        }

        if (!veterinario.Activo)
        {
            throw new ReglaNegocioException(
                "El veterinario seleccionado está inactivo.");
        }

        var existeConflicto = await _citaRepository.ExisteConflictoAsync(
            cita.VeterinarioId,
            cita.MascotaId,
            cita.FechaHora,
            cita.FechaHoraFin);

        if (existeConflicto)
        {
            throw new ReglaNegocioException(
                "El veterinario o la mascota ya tiene una cita en ese horario.");
        }

        await _citaRepository.AgregarAsync(cita);

        return cita.Id;
    }
}