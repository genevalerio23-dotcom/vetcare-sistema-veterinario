using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCare.Application.DTOs;
using VetCare.Application.Interfaces;
using VetCare.Domain.Exceptions;
using VetCare.Web.Models;

namespace VetCare.Web.Controllers;

[Authorize(Roles = "Administrador,Veterinario")]
public class TratamientosController : Controller
{
    private readonly ITratamientoService _tratamientoService;
    private readonly TimeProvider _reloj;
    private readonly ILogger<TratamientosController> _logger;

    public TratamientosController(
        ITratamientoService tratamientoService,
        TimeProvider reloj,
        ILogger<TratamientosController> logger)
    {
        _tratamientoService = tratamientoService;
        _reloj = reloj;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Crear(int? consultaId)
    {
        return View(new CrearTratamientoViewModel
        {
            ConsultaId = consultaId ?? 0,
            FechaInicio = _reloj.GetLocalNow().Date
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        CrearTratamientoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var id = await _tratamientoService.RegistrarAsync(
                new CrearTratamientoDto
                {
                    ConsultaId = model.ConsultaId,
                    Descripcion = model.Descripcion,
                    Indicaciones = model.Indicaciones,
                    FechaInicio = model.FechaInicio!.Value,
                    FechaFin = model.FechaFin
                });

            TempData["Exito"] =
                $"Tratamiento registrado correctamente. Código: {id}.";

            return RedirectToAction(
                nameof(Crear),
                new { consultaId = model.ConsultaId });
        }
        catch (ReglaNegocioException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar un tratamiento.");

            ModelState.AddModelError(
                string.Empty,
                "No se pudo registrar el tratamiento. Intenta nuevamente.");
        }

        return View(model);
    }
}