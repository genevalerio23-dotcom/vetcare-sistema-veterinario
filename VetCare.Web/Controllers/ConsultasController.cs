using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetCare.Application.DTOs;
using VetCare.Application.Interfaces;
using VetCare.Domain.Exceptions;
using VetCare.Web.Models;

namespace VetCare.Web.Controllers;

[Authorize(Roles = "Administrador,Veterinario")]
public class ConsultasController : Controller
{
    private readonly IConsultaService _consultaService;
    private readonly ICatalogoService _catalogoService;
    private readonly TimeProvider _reloj;
    private readonly ILogger<ConsultasController> _logger;

    public ConsultasController(
        IConsultaService consultaService,
        ICatalogoService catalogoService,
        TimeProvider reloj,
        ILogger<ConsultasController> logger)
    {
        _consultaService = consultaService;
        _catalogoService = catalogoService;
        _reloj = reloj;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        var model = new CrearConsultaViewModel
        {
            Fecha = _reloj.GetLocalNow().DateTime
        };

        await CargarOpcionesAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearConsultaViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var id = await _consultaService.RegistrarAsync(
                    new CrearConsultaDto
                    {
                        MascotaId = model.MascotaId,
                        VeterinarioId = model.VeterinarioId,
                        CitaId = model.CitaId,
                        Fecha = model.Fecha!.Value,
                        Motivo = model.Motivo,
                        Diagnostico = model.Diagnostico,
                        Observaciones = model.Observaciones ?? string.Empty
                    });

                TempData["Exito"] =
                    $"Consulta registrada correctamente. Código: {id}.";

                return RedirectToAction(nameof(Crear));
            }
            catch (ReglaNegocioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar una consulta.");

                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo registrar la consulta. Intenta nuevamente.");
            }
        }

        await CargarOpcionesAsync(model);

        return View(model);
    }

    private async Task CargarOpcionesAsync(
        CrearConsultaViewModel model)
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