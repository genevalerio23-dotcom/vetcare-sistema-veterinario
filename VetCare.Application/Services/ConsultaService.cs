using System;
using System.Threading.Tasks;
using VetCare.Application.DTOs;
using VetCare.Application.Interfaces;
using VetCare.Domain.Entities;
using VetCare.Domain.Exceptions;
using VetCare.Domain.Interfaces;

namespace VetCare.Application.Services;

public class ConsultaService : IConsultaService
{
    private readonly IConsultaRepository _consultaRepository;
    private readonly IMascotaRepository _mascotaRepository;
    private readonly IVeterinarioRepository _veterinarioRepository;
    private readonly ICitaRepository _citaRepository;
    private readonly TimeProvider _reloj;

    public ConsultaService(
        IConsultaRepository consultaRepository,
        IMascotaRepository mascotaRepository,
        IVeterinarioRepository veterinarioRepository,
        ICitaRepository citaRepository,
        TimeProvider reloj)
    {
        _consultaRepository = consultaRepository;
        _mascotaRepository = mascotaRepository;
        _veterinarioRepository = veterinarioRepository;
        _citaRepository = citaRepository;
        _reloj = reloj;
    }

    public async Task<int> RegistrarAsync(CrearConsultaDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var consulta = new Consulta(
            dto.MascotaId,
            dto.VeterinarioId,
            dto.CitaId,
            dto.Fecha,
            dto.Motivo,
            dto.Diagnostico,
            dto.Observaciones);

        var ahora = _reloj.GetLocalNow().DateTime;

        if (consulta.Fecha > ahora)
        {
            throw new ReglaNegocioException(
                "La consulta no puede registrarse con una fecha futura.");
        }

        var mascota = await _mascotaRepository.ObtenerPorIdAsync(
            consulta.MascotaId);

        if (mascota is null || !mascota.Activo)
        {
            throw new ReglaNegocioException(
                "Debe seleccionar una mascota existente y activa.");
        }

        var veterinario = await _veterinarioRepository.ObtenerPorIdAsync(
            consulta.VeterinarioId);

        if (veterinario is null || !veterinario.Activo)
        {
            throw new ReglaNegocioException(
                "Debe seleccionar un veterinario existente y activo.");
        }

        Cita? cita = null;

        if (consulta.CitaId.HasValue)
        {
            cita = await _citaRepository.ObtenerPorIdAsync(
                consulta.CitaId.Value);

            if (cita is null)
            {
                throw new ReglaNegocioException(
                    "La cita seleccionada no existe.");
            }

            if (cita.MascotaId != consulta.MascotaId ||
                cita.VeterinarioId != consulta.VeterinarioId)
            {
                throw new ReglaNegocioException(
                    "La cita no corresponde a la mascota y al veterinario seleccionados.");
            }

            if (cita.Estado != "Programada")
            {
                throw new ReglaNegocioException(
                    "Solo se puede registrar una consulta para una cita programada.");
            }

            if (consulta.Fecha < cita.FechaHora)
            {
                throw new ReglaNegocioException(
                    "La consulta no puede ser anterior al horario de su cita.");
            }

            if (await _consultaRepository.ExistePorCitaAsync(cita.Id))
            {
                throw new ReglaNegocioException(
                    "La cita ya tiene una consulta registrada.");
            }

            cita.MarcarAtendida();
        }

        await _consultaRepository.AgregarConCitaAsync(consulta, cita);

        return consulta.Id;
    }
}