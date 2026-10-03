using System.Threading.Tasks;
using VetCare.Application.DTOs;

namespace VetCare.Application.Interfaces;

public interface IMascotaService
{
    Task<int> RegistrarAsync(CrearMascotaDto dto);
}