using System.Data;
using Microsoft.EntityFrameworkCore;
using VetCare.Domain.Entities;
using VetCare.Domain.Exceptions;
using VetCare.Domain.Interfaces;
using VetCare.Infrastructure.Data;

namespace VetCare.Infrastructure.Repositories;

public class ConsultaRepository : IConsultaRepository
{
    private readonly VetCareDbContext _context;

    public ConsultaRepository(VetCareDbContext context)
    {
        _context = context;
    }

    public async Task<Consulta?> ObtenerPorIdAsync(int id)
    {
        return await _context.Consultas
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IReadOnlyList<Consulta>> ObtenerPorMascotaAsync(
        int mascotaId)
    {
        return await _context.Consultas
            .AsNoTracking()
            .Where(c => c.MascotaId == mascotaId)
            .OrderByDescending(c => c.Fecha)
            .ThenByDescending(c => c.Id)
            .ToListAsync();
    }

    public async Task<bool> ExistePorCitaAsync(int citaId)
    {
        return await _context.Consultas
            .AnyAsync(c => c.CitaId == citaId);
    }

    public async Task AgregarConCitaAsync(
        Consulta consulta,
        Cita? cita)
    {
        ArgumentNullException.ThrowIfNull(consulta);

        if (consulta.CitaId != cita?.Id)
        {
            throw new ReglaNegocioException(
                "La cita recibida no corresponde a la consulta.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        if (cita is not null)
        {
            var citaActual = await _context.Citas
                .FirstOrDefaultAsync(c => c.Id == cita.Id);

            if (citaActual is null)
            {
                throw new ReglaNegocioException(
                    "La cita ya no existe.");
            }

            await _context.Entry(citaActual).ReloadAsync();

            if (citaActual.Estado != "Programada")
            {
                throw new ReglaNegocioException(
                    "La cita ya fue atendida o cancelada.");
            }

            if (citaActual.MascotaId != consulta.MascotaId ||
                citaActual.VeterinarioId != consulta.VeterinarioId)
            {
                throw new ReglaNegocioException(
                    "La mascota o el veterinario no coincide con la cita.");
            }

            if (consulta.Fecha < citaActual.FechaHora)
            {
                throw new ReglaNegocioException(
                    "La consulta no puede ser anterior a la cita.");
            }

            if (await ExistePorCitaAsync(citaActual.Id))
            {
                throw new ReglaNegocioException(
                    "Esta cita ya tiene una consulta registrada.");
            }

            citaActual.MarcarAtendida();
        }

        await _context.Consultas.AddAsync(consulta);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();
    }
}