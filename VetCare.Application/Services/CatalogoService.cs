using VetCare.Application.DTOs;
using VetCare.Application.Interfaces;
using VetCare.Domain.Interfaces;

namespace VetCare.Application.Services;

public class CatalogoService : ICatalogoService
{
    private readonly IMascotaRepository _mascotaRepository;
    private readonly IVeterinarioRepository _veterinarioRepository;
    private readonly IClienteRepository _clienteRepository;

    public CatalogoService(
        IMascotaRepository mascotaRepository,
        IVeterinarioRepository veterinarioRepository,
        IClienteRepository clienteRepository)
    {
        _mascotaRepository = mascotaRepository;
        _veterinarioRepository = veterinarioRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<IReadOnlyList<OpcionSeleccionDto>>
        ObtenerMascotasActivasAsync()
    {
        var mascotas =
            await _mascotaRepository.ObtenerTodasAsync();

        var clientes =
            await _clienteRepository.ObtenerTodosAsync();

        var propietarios = clientes.ToDictionary(
            c => c.Id,
            c => $"{c.Nombres} {c.Apellidos}");

        return mascotas
            .Where(m => m.Activo)
            .Select(m => new OpcionSeleccionDto
            {
                Id = m.Id,
                Texto = propietarios.TryGetValue(
                    m.ClienteId, out var propietario)
                    ? $"{m.Nombre} — {propietario} (Código: {m.Id})"
                    : $"{m.Nombre} (Código: {m.Id})"
            })
            .ToList();
    }

    public async Task<IReadOnlyList<OpcionSeleccionDto>>
        ObtenerVeterinariosActivosAsync()
    {
        var veterinarios =
            await _veterinarioRepository.ObtenerTodosAsync();

        return veterinarios
            .Where(v => v.Activo)
            .Select(v => new OpcionSeleccionDto
            {
                Id = v.Id,
                Texto = $"{v.Nombre} — {v.Especialidad}"
            })
            .ToList();
    }
}