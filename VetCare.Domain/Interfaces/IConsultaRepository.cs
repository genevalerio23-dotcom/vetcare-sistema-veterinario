using System.Collections.Generic;
using System.Threading.Tasks;
using VetCare.Domain.Entities;

namespace VetCare.Domain.Interfaces;

public interface IConsultaRepository
{
    Task<Consulta?> ObtenerPorIdAsync(int id);
    Task<IReadOnlyList<Consulta>> ObtenerPorMascotaAsync(int mascotaId);
    Task<bool> ExistePorCitaAsync(int citaId);

    Task AgregarConCitaAsync(
        Consulta consulta,
        Cita? cita);
}