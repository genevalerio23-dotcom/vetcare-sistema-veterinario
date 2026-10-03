using VetCare.Domain.Exceptions;

namespace VetCare.Domain.Entities;

public class Mascota
{
    public int Id { get; private set; }
    public int ClienteId { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Especie { get; private set; } = string.Empty;
    public string Raza { get; private set; } = string.Empty;
    public string Sexo { get; private set; } = string.Empty;
    public decimal Peso { get; private set; }
    public bool Activo { get; private set; } = true;

    private Mascota()
    {
    }

    public Mascota(
        int clienteId,
        string nombre,
        string especie,
        string raza,
        string sexo,
        decimal peso)
    {
        if (clienteId <= 0)
        {
            throw new ReglaNegocioException(
                "Debe seleccionar un propietario válido.");
        }

        ActualizarDatos(nombre, especie, raza, sexo, peso);
        ClienteId = clienteId;
    }

    public void ActualizarDatos(
        string nombre,
        string especie,
        string raza,
        string sexo,
        decimal peso)
    {
        var nombreLimpio = (nombre ?? string.Empty).Trim();
        var especieLimpia = (especie ?? string.Empty).Trim();
        var razaLimpia = (raza ?? string.Empty).Trim();
        var sexoLimpio = (sexo ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(nombreLimpio) ||
            nombreLimpio.Length > 100)
        {
            throw new ReglaNegocioException(
                "El nombre es obligatorio y no debe superar 100 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(especieLimpia) ||
            especieLimpia.Length > 50)
        {
            throw new ReglaNegocioException(
                "La especie es obligatoria y no debe superar 50 caracteres.");
        }

        if (razaLimpia.Length > 100)
        {
            throw new ReglaNegocioException(
                "La raza no debe superar 100 caracteres.");
        }

        if (sexoLimpio != "Macho" && sexoLimpio != "Hembra")
        {
            throw new ReglaNegocioException(
                "El sexo debe ser Macho o Hembra.");
        }

        if (peso <= 0)
        {
            throw new ReglaNegocioException(
                "El peso debe ser mayor que cero.");
        }

        if (peso > 9999.99m || decimal.Round(peso, 2) != peso)
        {
            throw new ReglaNegocioException(
                "El peso no debe superar 9999.99 kg y admite hasta dos decimales.");
        }

        Nombre = nombreLimpio;
        Especie = especieLimpia;
        Raza = razaLimpia;
        Sexo = sexoLimpio;
        Peso = peso;
    }

    public void Desactivar()
    {
        Activo = false;
    }
}