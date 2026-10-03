using System.Collections.Generic;
using System.Threading.Tasks;
using VetCare.Domain.Entities;

namespace VetCare.Domain.Interfaces;

public interface ITratamientoRepository
{
    Task<IReadOnlyList<Tratamiento>> ObtenerPorConsultaAsync(int consultaId);
    Task AgregarAsync(Tratamiento tratamiento);
}