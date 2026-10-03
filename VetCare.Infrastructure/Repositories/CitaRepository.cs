using System.Data;
using Microsoft.EntityFrameworkCore;
using VetCare.Domain.Entities;
using VetCare.Domain.Exceptions;
using VetCare.Domain.Interfaces;
using VetCare.Infrastructure.Data;

namespace VetCare.Infrastructure.Repositories;

public class CitaRepository : ICitaRepository
{
    private readonly VetCareDbContext _context;

    public CitaRepository(VetCareDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Cita>> ObtenerTodasAsync()
    {
        return await _context.Citas
            .AsNoTracking()
            .OrderBy(c => c.FechaHora)
            .ToListAsync();
    }

    public async Task<Cita?> ObtenerPorIdAsync(int id)
    {
        return await _context.Citas
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> ExisteConflictoAsync(
        int veterinarioId,
        int mascotaId,
        DateTime inicio,
        DateTime fin)
    {
        return await _context.Citas.AnyAsync(c =>
            c.Estado != "Cancelada" &&
            (c.VeterinarioId == veterinarioId ||
             c.MascotaId == mascotaId) &&
            c.FechaHora < fin &&
            c.FechaHora.AddMinutes(30) > inicio);
    }

    public async Task AgregarAsync(Cita cita)
    {
        ArgumentNullException.ThrowIfNull(cita);

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        var existeConflicto = await ExisteConflictoAsync(
            cita.VeterinarioId,
            cita.MascotaId,
            cita.FechaHora,
            cita.FechaHoraFin);

        if (existeConflicto)
        {
            throw new ReglaNegocioException(
                "El veterinario o la mascota ya tiene una cita en ese horario.");
        }

        await _context.Citas.AddAsync(cita);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();
    }
}
