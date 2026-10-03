using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCare.Application.DTOs;
using VetCare.Application.Interfaces;
using VetCare.Domain.Exceptions;
using VetCare.Web.Models;

namespace VetCare.Web.Controllers;

[Authorize(Roles = "Administrador,Recepcionista")]
public class ClientesController : Controller
{
    private readonly IClienteService _clienteService;
    private readonly ILogger<ClientesController> _logger;

    public ClientesController(
        IClienteService clienteService,
        ILogger<ClientesController> logger)
    {
        _clienteService = clienteService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View(new CrearClienteViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CrearClienteViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var id = await _clienteService.RegistrarAsync(
                new CrearClienteDto
                {
                    Nombres = model.Nombres,
                    Apellidos = model.Apellidos,
                    Telefono = model.Telefono,
                    Email = model.Email ?? string.Empty,
                    Direccion = model.Direccion ?? string.Empty
                });

            TempData["Exito"] = $"Cliente registrado correctamente. Código: {id}.";

            return RedirectToAction(nameof(Crear));
        }
        catch (ReglaNegocioException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar un cliente.");

            ModelState.AddModelError(
                string.Empty,
                "No se pudo registrar el cliente. Intenta nuevamente.");
        }

        return View(model);
    }
}