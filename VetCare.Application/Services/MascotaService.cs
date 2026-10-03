using System;
using System.Threading.Tasks;
using VetCare.Application.DTOs;
using VetCare.Application.Interfaces;
using VetCare.Domain.Entities;
using VetCare.Domain.Exceptions;
using VetCare.Domain.Interfaces;

namespace VetCare.Application.Services;

public class MascotaService : IMascotaService
{
    private readonly IMascotaRepository _mascotaRepository;
    private readonly IClienteRepository _clienteRepository;

    public MascotaService(
        IMascotaRepository mascotaRepository,
        IClienteRepository clienteRepository)
    {
        _mascotaRepository = mascotaRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<int> RegistrarAsync(CrearMascotaDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var mascota = new Mascota(
            dto.ClienteId,
            dto.Nombre,
            dto.Especie,
            dto.Raza,
            dto.Sexo,
            dto.Peso);

        var cliente = await _clienteRepository.ObtenerPorIdAsync(
            dto.ClienteId);

        if (cliente is null)
        {
            throw new ReglaNegocioException(
                "El propietario seleccionado no existe.");
        }

        if (!cliente.Activo)
        {
            throw new ReglaNegocioException(
                "No se pueden registrar mascotas para un cliente inactivo.");
        }

        await _mascotaRepository.AgregarAsync(mascota);

        return mascota.Id;
    }
}