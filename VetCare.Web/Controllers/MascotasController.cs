using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetCare.Application.DTOs;
using VetCare.Application.Interfaces;
using VetCare.Domain.Exceptions;
using VetCare.Web.Models;

namespace VetCare.Web.Controllers;

[Authorize(Roles = "Administrador,Recepcionista")]
public class MascotasController : Controller
{
    private readonly IMascotaService _mascotaService;
    private readonly IClienteService _clienteService;
    private readonly ILogger<MascotasController> _logger;

    public MascotasController(
        IMascotaService mascotaService,
        IClienteService clienteService,
        ILogger<MascotasController> logger)
    {
        _mascotaService = mascotaService;
        _clienteService = clienteService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Crear()
    {
        var model = new CrearMascotaViewModel();

        await CargarPropietariosAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearMascotaViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var id = await _mascotaService.RegistrarAsync(
                    new CrearMascotaDto
                    {
                        ClienteId = model.ClienteId,
                        Nombre = model.Nombre,
                        Especie = model.Especie,
                        Raza = model.Raza ?? string.Empty,
                        Sexo = model.Sexo,
                        Peso = model.Peso
                    });

                TempData["Exito"] =
                    $"Mascota registrada correctamente. Código: {id}.";

                return RedirectToAction(nameof(Crear));
            }
            catch (ReglaNegocioException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar una mascota.");

                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo registrar la mascota. Intenta nuevamente.");
            }
        }

        await CargarPropietariosAsync(model);

        return View(model);
    }

    private async Task CargarPropietariosAsync(
        CrearMascotaViewModel model)
    {
        var propietarios = await _clienteService.ObtenerActivosAsync();

        model.Propietarios = propietarios
            .Select(cliente => new SelectListItem
            {
                Value = cliente.Id.ToString(),
                Text = $"{cliente.NombreCompleto} (Código: {cliente.Id})"
            })
            .ToList();
    }
}