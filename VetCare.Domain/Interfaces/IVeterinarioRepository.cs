using System.Collections.Generic;
using System.Threading.Tasks;
using VetCare.Domain.Entities;

namespace VetCare.Domain.Interfaces;

public interface IVeterinarioRepository
{
    Task<IReadOnlyList<Veterinario>> ObtenerTodosAsync();
    Task<Veterinario?> ObtenerPorIdAsync(int id);
}