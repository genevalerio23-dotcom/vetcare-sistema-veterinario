using System;
using System.Threading.Tasks;
using VetCare.Application.DTOs;
using VetCare.Application.Interfaces;
using VetCare.Domain.Entities;
using VetCare.Domain.Exceptions;
using VetCare.Domain.Interfaces;

namespace VetCare.Application.Services;

public class TratamientoService : ITratamientoService
{
    private readonly ITratamientoRepository _tratamientoRepository;
    private readonly IConsultaRepository _consultaRepository;

    public TratamientoService(
        ITratamientoRepository tratamientoRepository,
        IConsultaRepository consultaRepository)
    {
        _tratamientoRepository = tratamientoRepository;
        _consultaRepository = consultaRepository;
    }

    public async Task<int> RegistrarAsync(CrearTratamientoDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var tratamiento = new Tratamiento(
            dto.ConsultaId,
            dto.Descripcion,
            dto.Indicaciones,
            dto.FechaInicio,
            dto.FechaFin);

        var consulta = await _consultaRepository.ObtenerPorIdAsync(
            tratamiento.ConsultaId);

        if (consulta is null)
        {
            throw new ReglaNegocioException(
                "La consulta seleccionada no existe.");
        }

        if (tratamiento.FechaInicio < consulta.Fecha.Date)
        {
            throw new ReglaNegocioException(
                "El tratamiento no puede iniciar antes de la fecha de la consulta.");
        }

        await _tratamientoRepository.AgregarAsync(tratamiento);

        return tratamiento.Id;
    }
}