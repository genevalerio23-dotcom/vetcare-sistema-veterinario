using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetCare.Application.DTOs;
using VetCare.Application.Interfaces;
using VetCare.Domain.Exceptions;
using VetCare.Web.Models;

namespace VetCare.Web.Controllers;

[Authorize(Roles = "Administrador,Recepcionista")]
public class CitasController : Controller
{
    private readonly ICitaService _citaService;
    private readonly ICatalogoService _catalogoService;
    private readonly ILogger<CitasController> _logger;

    public CitasController(
        ICitaService citaService,
        ICatalogoService catalogoService,
        ILogger<CitasController> logger)
    {
        _citaService = citaService;
        _catalogoService = catalogoService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        var model = new CrearCitaViewModel();

        await CargarOpcionesAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearCitaViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var id = await _citaService.AgendarAsync(
                    new CrearCitaDto
                    {
                        MascotaId = model.MascotaId,
                        VeterinarioId = model.VeterinarioId,
                        FechaHora = model.FechaHora!.Value,
                        Motivo = model.Motivo
                    });

                TempData["Exito"] =
                    $"Cita registrada correctamente. Código: {id}.";

                return RedirectToAction(nameof(Crear));
            }
            catch (ReglaNegocioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar una cita.");

                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo registrar la cita. Intenta nuevamente.");
            }
        }

        await CargarOpcionesAsync(model);

        return View(model);
    }

    private async Task CargarOpcionesAsync(CrearCitaViewModel model)
    {
        var mascotas = await _catalogoService.ObtenerMascotasActivasAsync();

        model.Mascotas = mascotas
            .Select(mascota => new SelectListItem
            {
                Value = mascota.Id.ToString(),
                Text = mascota.Texto
            })
            .ToList();

        var veterinarios =
            await _catalogoService.ObtenerVeterinariosActivosAsync();

        model.Veterinarios = veterinarios
            .Select(veterinario => new SelectListItem
            {
                Value = veterinario.Id.ToString(),
                Text = veterinario.Texto
            })
            .ToList();
    }
}