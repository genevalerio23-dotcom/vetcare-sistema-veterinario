using VetCare.Application.DTOs;
using VetCare.Application.Interfaces;
using VetCare.Domain.Entities;
using VetCare.Domain.Interfaces;

namespace VetCare.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<int> RegistrarAsync(CrearClienteDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var cliente = new Cliente(
            dto.Nombres,
            dto.Apellidos,
            dto.Telefono,
            dto.Email,
            dto.Direccion);

        await _clienteRepository.AgregarAsync(cliente);

        return cliente.Id;
    }

    public async Task<IReadOnlyList<ClienteSeleccionDto>>
        ObtenerActivosAsync()
    {
        var clientes = await _clienteRepository.ObtenerTodosAsync();

        return clientes
            .Where(c => c.Activo)
            .Select(c => new ClienteSeleccionDto
            {
                Id = c.Id,
                NombreCompleto = $"{c.Nombres} {c.Apellidos}"
            })
            .ToList();
    }
}