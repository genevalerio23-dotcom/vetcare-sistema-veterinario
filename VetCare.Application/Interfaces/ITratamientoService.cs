using System.Threading.Tasks;
using VetCare.Application.DTOs;

namespace VetCare.Application.Interfaces;

public interface ITratamientoService
{
    Task<int> RegistrarAsync(CrearTratamientoDto dto);
}