using System.Threading.Tasks;
using VetCare.Application.DTOs;

namespace VetCare.Application.Interfaces;

public interface IHistorialService
{
    Task<HistorialMascotaDto> ObtenerPorMascotaAsync(int mascotaId);
}