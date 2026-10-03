using System.Collections.Generic;
using System.Threading.Tasks;
using VetCare.Domain.Entities;

namespace VetCare.Domain.Interfaces;

public interface IMascotaRepository
{
    Task<IReadOnlyList<Mascota>> ObtenerTodasAsync();
    Task<Mascota?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Mascota mascota);
}