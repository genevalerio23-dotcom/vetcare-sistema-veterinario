using VetCare.Application.DTOs;

namespace VetCare.Application.Interfaces;

public interface ICatalogoService
{
    Task<IReadOnlyList<OpcionSeleccionDto>>
        ObtenerMascotasActivasAsync();

    Task<IReadOnlyList<OpcionSeleccionDto>>
        ObtenerVeterinariosActivosAsync();
}