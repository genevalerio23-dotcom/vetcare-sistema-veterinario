using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VetCare.Domain.Entities;

namespace VetCare.Domain.Interfaces;

public interface ICitaRepository
{
    Task<IReadOnlyList<Cita>> ObtenerTodasAsync();
    Task<Cita?> ObtenerPorIdAsync(int id);

    Task<bool> ExisteConflictoAsync(
        int veterinarioId,
        int mascotaId,
        DateTime inicio,
        DateTime fin);

    Task AgregarAsync(Cita cita);
}