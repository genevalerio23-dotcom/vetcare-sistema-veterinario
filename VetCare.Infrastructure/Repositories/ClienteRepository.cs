using Microsoft.EntityFrameworkCore;
using VetCare.Domain.Entities;
using VetCare.Domain.Interfaces;
using VetCare.Infrastructure.Data;

namespace VetCare.Infrastructure.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly VetCareDbContext _context;

    public ClienteRepository(VetCareDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Cliente>> ObtenerTodosAsync()
    {
        return await _context.Clientes
            .AsNoTracking()
            .OrderBy(c => c.Nombres)
            .ThenBy(c => c.Apellidos)
            .ToListAsync();
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id)
    {
        return await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AgregarAsync(Cliente cliente)
    {
        ArgumentNullException.ThrowIfNull(cliente);

        await _context.Clientes.AddAsync(cliente);
        await _context.SaveChangesAsync();
    }
}
