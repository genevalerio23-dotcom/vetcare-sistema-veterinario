using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VetCare.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace VetCare.Web.Controllers;

public class CuentaController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public CuentaController(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel
        {
            ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : null
        });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        model.ReturnUrl = Url.IsLocalUrl(model.ReturnUrl)
            ? model.ReturnUrl
            : null;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.Email.Trim();
        var usuario = await _signInManager.UserManager.FindByEmailAsync(email);

        if (usuario == null || string.IsNullOrWhiteSpace(usuario.UserName))
        {
            ModelState.AddModelError(
                string.Empty,
                "Correo o contraseña incorrectos.");

            return View(model);
        }

        var resultado = await _signInManager.PasswordSignInAsync(
            usuario.UserName,
            model.Password,
            model.Recordarme,
            lockoutOnFailure: true);

        if (resultado.Succeeded)
        {
            if (!string.IsNullOrEmpty(model.ReturnUrl))
            {
                return LocalRedirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(
            string.Empty,
            resultado.IsLockedOut
                ? "Cuenta bloqueada temporalmente. Intenta nuevamente en 5 minutos."
                : "Correo o contraseña incorrectos.");

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccesoDenegado()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;

        return View();
    }
}
