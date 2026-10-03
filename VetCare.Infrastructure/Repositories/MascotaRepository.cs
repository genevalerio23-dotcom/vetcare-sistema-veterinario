using Microsoft.EntityFrameworkCore;
using VetCare.Domain.Entities;
using VetCare.Domain.Interfaces;
using VetCare.Infrastructure.Data;

namespace VetCare.Infrastructure.Repositories;

public class MascotaRepository : IMascotaRepository
{
    private readonly VetCareDbContext _context;

    public MascotaRepository(VetCareDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Mascota>> ObtenerTodasAsync()
    {
        return await _context.Mascotas
            .AsNoTracking()
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }

    public async Task<Mascota?> ObtenerPorIdAsync(int id)
    {
        return await _context.Mascotas
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task AgregarAsync(Mascota mascota)
    {
        ArgumentNullException.ThrowIfNull(mascota);

        await _context.Mascotas.AddAsync(mascota);
        await _context.SaveChangesAsync();
    }
}