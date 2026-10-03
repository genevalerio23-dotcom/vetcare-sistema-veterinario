using System.Net.Mail;
using VetCare.Domain.Exceptions;

namespace VetCare.Domain.Entities;

public class Cliente
{
    public int Id { get; private set; }
    public string Nombres { get; private set; } = string.Empty;
    public string Apellidos { get; private set; } = string.Empty;
    public string Telefono { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Direccion { get; private set; } = string.Empty;
    public bool Activo { get; private set; } = true;

    private Cliente()
    {
    }

    public Cliente(
        string nombres,
        string apellidos,
        string telefono,
        string email,
        string direccion)
    {
        ActualizarDatos(nombres, apellidos, telefono, email, direccion);
    }

    public void ActualizarDatos(
        string nombres,
        string apellidos,
        string telefono,
        string email,
        string direccion)
    {
        var nombresLimpios = (nombres ?? string.Empty).Trim();
        var apellidosLimpios = (apellidos ?? string.Empty).Trim();
        var telefonoLimpio = (telefono ?? string.Empty).Trim();
        var emailLimpio = (email ?? string.Empty).Trim();
        var direccionLimpia = (direccion ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(nombresLimpios) ||
            nombresLimpios.Length > 100)
        {
            throw new ReglaNegocioException(
                "Los nombres son obligatorios y no deben superar 100 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(apellidosLimpios) ||
            apellidosLimpios.Length > 100)
        {
            throw new ReglaNegocioException(
                "Los apellidos son obligatorios y no deben superar 100 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(telefonoLimpio) ||
            telefonoLimpio.Length > 20)
        {
            throw new ReglaNegocioException(
                "El teléfono es obligatorio y no debe superar 20 caracteres.");
        }

        if (emailLimpio.Length > 150 ||
            (emailLimpio.Length > 0 &&
             (!MailAddress.TryCreate(emailLimpio, out var correo) ||
              correo.Address != emailLimpio)))
        {
            throw new ReglaNegocioException(
                "El correo electrónico debe ser válido y no superar 150 caracteres.");
        }

        if (direccionLimpia.Length > 250)
        {
            throw new ReglaNegocioException(
                "La dirección no debe superar 250 caracteres.");
        }

        Nombres = nombresLimpios;
        Apellidos = apellidosLimpios;
        Telefono = telefonoLimpio;
        Email = emailLimpio;
        Direccion = direccionLimpia;
    }

    public void Desactivar()
    {
        Activo = false;
    }
}