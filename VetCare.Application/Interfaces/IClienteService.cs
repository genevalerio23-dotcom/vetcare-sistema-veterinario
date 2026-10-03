using VetCare.Application.DTOs;

namespace VetCare.Application.Interfaces;

public interface IClienteService
{
    Task<int> RegistrarAsync(CrearClienteDto dto);

    Task<IReadOnlyList<ClienteSeleccionDto>> ObtenerActivosAsync();
}