using System.Linq;
using System.Threading.Tasks;
using VetCare.Application.DTOs;
using VetCare.Application.Interfaces;
using VetCare.Domain.Exceptions;
using VetCare.Domain.Interfaces;

namespace VetCare.Application.Services;

public class HistorialService : IHistorialService
{
    private readonly IMascotaRepository _mascotaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IConsultaRepository _consultaRepository;
    private readonly IVeterinarioRepository _veterinarioRepository;
    private readonly ITratamientoRepository _tratamientoRepository;

    public HistorialService(
        IMascotaRepository mascotaRepository,
        IClienteRepository clienteRepository,
        IConsultaRepository consultaRepository,
        IVeterinarioRepository veterinarioRepository,
        ITratamientoRepository tratamientoRepository)
    {
        _mascotaRepository = mascotaRepository;
        _clienteRepository = clienteRepository;
        _consultaRepository = consultaRepository;
        _veterinarioRepository = veterinarioRepository;
        _tratamientoRepository = tratamientoRepository;
    }

    public async Task<HistorialMascotaDto> ObtenerPorMascotaAsync(
        int mascotaId)
    {
        if (mascotaId <= 0)
        {
            throw new ReglaNegocioException(
                "Debe seleccionar una mascota válida.");
        }

        var mascota = await _mascotaRepository.ObtenerPorIdAsync(
            mascotaId);

        if (mascota is null)
        {
            throw new ReglaNegocioException(
                "No se encontró la mascota solicitada.");
        }

        var cliente = await _clienteRepository.ObtenerPorIdAsync(
            mascota.ClienteId);

        if (cliente is null)
        {
            throw new ReglaNegocioException(
                "No se encontró el propietario de la mascota.");
        }

        var historial = new HistorialMascotaDto
        {
            MascotaId = mascota.Id,
            Nombre = mascota.Nombre,
            Especie = mascota.Especie,
            Raza = mascota.Raza,
            Sexo = mascota.Sexo,
            Peso = mascota.Peso,
            Propietario = $"{cliente.Nombres} {cliente.Apellidos}"
        };

        var consultas = await _consultaRepository.ObtenerPorMascotaAsync(
            mascotaId);

        foreach (var consulta in consultas
                     .OrderByDescending(c => c.Fecha)
                     .ThenByDescending(c => c.Id))
        {
            var veterinario =
                await _veterinarioRepository.ObtenerPorIdAsync(
                    consulta.VeterinarioId);

            if (veterinario is null)
            {
                throw new ReglaNegocioException(
                    "No se encontró el veterinario de una consulta.");
            }

            var tratamientos =
                await _tratamientoRepository.ObtenerPorConsultaAsync(
                    consulta.Id);

            historial.Consultas.Add(new ConsultaHistorialDto
            {
                Id = consulta.Id,
                CitaId = consulta.CitaId,
                Fecha = consulta.Fecha,
                Veterinario = veterinario.Nombre,
                Motivo = consulta.Motivo,
                Diagnostico = consulta.Diagnostico,
                Observaciones = consulta.Observaciones,
                Tratamientos = tratamientos
                    .OrderBy(t => t.FechaInicio)
                    .ThenBy(t => t.Id)
                    .Select(t => new TratamientoDto
                    {
                        Id = t.Id,
                        Descripcion = t.Descripcion,
                        Indicaciones = t.Indicaciones,
                        FechaInicio = t.FechaInicio,
                        FechaFin = t.FechaFin
                    })
                    .ToList()
            });
        }

        return historial;
    }
}