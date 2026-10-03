using VetCare.Domain.Exceptions;

namespace VetCare.Domain.Entities;

public class Veterinario
{
    public int Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Especialidad { get; private set; } = string.Empty;
    public string Telefono { get; private set; } = string.Empty;
    public bool Activo { get; private set; } = true;

    private Veterinario()
    {
    }

    public Veterinario(
        string nombre,
        string especialidad,
        string telefono)
    {
        ActualizarDatos(nombre, especialidad, telefono);
    }

    public void ActualizarDatos(
        string nombre,
        string especialidad,
        string telefono)
    {
        var nombreLimpio = (nombre ?? string.Empty).Trim();
        var especialidadLimpia = (especialidad ?? string.Empty).Trim();
        var telefonoLimpio = (telefono ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(nombreLimpio) ||
            nombreLimpio.Length > 150)
        {
            throw new ReglaNegocioException(
                "El nombre es obligatorio y no debe superar 150 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(especialidadLimpia) ||
            especialidadLimpia.Length > 100)
        {
            throw new ReglaNegocioException(
                "La especialidad es obligatoria y no debe superar 100 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(telefonoLimpio) ||
            telefonoLimpio.Length > 20)
        {
            throw new ReglaNegocioException(
                "El teléfono es obligatorio y no debe superar 20 caracteres.");
        }

        Nombre = nombreLimpio;
        Especialidad = especialidadLimpia;
        Telefono = telefonoLimpio;
    }

    public void Desactivar()
    {
        Activo = false;
    }
}