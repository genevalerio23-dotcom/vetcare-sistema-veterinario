using System.Collections.Generic;
using System.Threading.Tasks;
using VetCare.Domain.Entities;

namespace VetCare.Domain.Interfaces;

public interface IClienteRepository
{
    Task<IReadOnlyList<Cliente>> ObtenerTodosAsync();
    Task<Cliente?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(Cliente cliente);
}