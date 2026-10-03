using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetCare.Application.Interfaces;
using VetCare.Domain.Exceptions;
using VetCare.Web.Models;

namespace VetCare.Web.Controllers;

[Authorize(Roles = "Administrador,Veterinario")]
public class HistorialController : Controller
{
    private readonly IHistorialService _historialService;
    private readonly ICatalogoService _catalogoService;
    private readonly ILogger<HistorialController> _logger;

    public HistorialController(
        IHistorialService historialService,
        ICatalogoService catalogoService,
        ILogger<HistorialController> logger)
    {
        _historialService = historialService;
        _catalogoService = catalogoService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? mascotaId)
    {
        var model = new HistorialViewModel
        {
            MascotaId = mascotaId
        };

        try
        {
            var mascotas =
                await _catalogoService.ObtenerMascotasActivasAsync();

            model.Mascotas = mascotas
                .Select(mascota => new SelectListItem
                {
                    Value = mascota.Id.ToString(),
                    Text = mascota.Texto
                })
                .ToList();

            if (ModelState.IsValid && mascotaId.HasValue)
            {
                model.Historial =
                    await _historialService.ObtenerPorMascotaAsync(
                        mascotaId.Value);
            }
        }
        catch (ReglaNegocioException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar el historial.");

            ModelState.AddModelError(
                string.Empty,
                "No se pudo cargar el historial. Intenta nuevamente.");
        }

        return View(model);
    }
}