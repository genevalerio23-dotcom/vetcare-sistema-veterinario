using Microsoft.AspNetCore.Identity;

namespace VetCare.Web.Security;

public static class IdentitySeeder
{
    public static async Task InicializarAsync(IServiceProvider servicios)
    {
        using var scope = servicios.CreateScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        string[] roles =
        {
            "Administrador",
            "Recepcionista",
            "Veterinario"
        };

        foreach (var rol in roles)
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                var resultado = await roleManager.CreateAsync(
                    new IdentityRole(rol));

                VerificarResultado(resultado, $"crear el rol {rol}");
            }
        }

        await CrearUsuarioAsync(
            userManager,
            "admin@vetcare.local",
            "VetCareAdmin2026!",
            "Administrador");

        await CrearUsuarioAsync(
            userManager,
            "recepcion@vetcare.local",
            "VetCareRecepcion2026!",
            "Recepcionista");

        await CrearUsuarioAsync(
            userManager,
            "veterinario@vetcare.local",
            "VetCareVeterinario2026!",
            "Veterinario");
    }

    private static async Task CrearUsuarioAsync(
        UserManager<IdentityUser> userManager,
        string email,
        string password,
        string rol)
    {
        var usuario = await userManager.FindByEmailAsync(email);

        if (usuario == null)
        {
            usuario = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var resultado = await userManager.CreateAsync(
                usuario,
                password);

            VerificarResultado(resultado, $"crear el usuario {email}");
        }

        if (!await userManager.IsInRoleAsync(usuario, rol))
        {
            var resultado = await userManager.AddToRoleAsync(
                usuario,
                rol);

            VerificarResultado(
                resultado,
                $"asignar el rol {rol} a {email}");
        }
    }

    private static void VerificarResultado(
        IdentityResult resultado,
        string operacion)
    {
        if (resultado.Succeeded)
        {
            return;
        }

        var errores = string.Join(
            "; ",
            resultado.Errors.Select(error => error.Description));

        throw new InvalidOperationException(
            $"No se pudo {operacion}: {errores}");
    }
}