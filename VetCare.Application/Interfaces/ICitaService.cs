using System.Threading.Tasks;
using VetCare.Application.DTOs;

namespace VetCare.Application.Interfaces;

public interface ICitaService
{
    Task<int> AgendarAsync(CrearCitaDto dto);
}
