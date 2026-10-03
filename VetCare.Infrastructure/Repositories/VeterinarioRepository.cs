using Microsoft.EntityFrameworkCore;
using VetCare.Domain.Entities;
using VetCare.Domain.Interfaces;
using VetCare.Infrastructure.Data;

namespace VetCare.Infrastructure.Repositories;

public class VeterinarioRepository : IVeterinarioRepository
{
    private readonly VetCareDbContext _context;

    public VeterinarioRepository(VetCareDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Veterinario>> ObtenerTodosAsync()
    {
        return await _context.Veterinarios
            .AsNoTracking()
            .OrderBy(v => v.Nombre)
            .ToListAsync();
    }

    public async Task<Veterinario?> ObtenerPorIdAsync(int id)
    {
        return await _context.Veterinarios
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);
    }
}
