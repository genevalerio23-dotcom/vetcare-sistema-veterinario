using Microsoft.EntityFrameworkCore;
using VetCare.Domain.Entities;
using VetCare.Domain.Interfaces;
using VetCare.Infrastructure.Data;

namespace VetCare.Infrastructure.Repositories;

public class TratamientoRepository : ITratamientoRepository
{
    private readonly VetCareDbContext _context;

    public TratamientoRepository(VetCareDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Tratamiento>>
        ObtenerPorConsultaAsync(int consultaId)
    {
        return await _context.Tratamientos
            .AsNoTracking()
            .Where(t => t.ConsultaId == consultaId)
            .OrderBy(t => t.FechaInicio)
            .ThenBy(t => t.Id)
            .ToListAsync();
    }

    public async Task AgregarAsync(Tratamiento tratamiento)
    {
        ArgumentNullException.ThrowIfNull(tratamiento);

        await _context.Tratamientos.AddAsync(tratamiento);
        await _context.SaveChangesAsync();
    }
}