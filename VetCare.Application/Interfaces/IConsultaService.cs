using System.Threading.Tasks;
using VetCare.Application.DTOs;

namespace VetCare.Application.Interfaces;

public interface IConsultaService
{
    Task<int> RegistrarAsync(CrearConsultaDto dto);
}