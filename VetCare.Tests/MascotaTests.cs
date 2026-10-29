
using VetCare.Domain.Entities;
using VetCare.Domain.Exceptions;
using Xunit;

namespace VetCare.Tests;

public class MascotaTests
{
    [Fact]
    public void RegistrarMascota_ConPesoValido_DebeCrearse()
    {
        // Crear una mascota con datos válidos
        var mascota = new Mascota(
            1, "Firulais", "Perro", "Labrador", "Macho", 5m);

        // Comprobar los datos registrados
        Assert.Equal("Firulais", mascota.Nombre);
        Assert.Equal(5m, mascota.Peso);
        Assert.Equal(1, mascota.ClienteId);
        Assert.True(mascota.Activo);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void RegistrarMascota_ConPesoInvalido_DebeRechazarse(
        int peso)
    {
        // Verificar que el dominio rechace pesos inválidos
        var excepcion = Assert.Throws<ReglaNegocioException>(
            () => new Mascota(
                1, "Firulais", "Perro", "Labrador", "Macho", peso));

        Assert.Equal(
            "El peso debe ser mayor que cero.",
            excepcion.Message);
    }

    [Fact]
    public void RegistrarMascota_SinPropietario_DebeRechazarse()
    {
        // Comprobar que se requiere un propietario válido
        Assert.Throws<ReglaNegocioException>(
            () => new Mascota(
                0, "Firulais", "Perro", "Labrador", "Macho", 5m));
    }

    [Fact]
    public void RegistrarMascota_SinNombre_DebeRechazarse()
    {
        // Comprobar que el nombre sea obligatorio
        Assert.Throws<ReglaNegocioException>(
            () => new Mascota(
                1, "", "Perro", "Labrador", "Macho", 5m));
    }

    [Fact]
    public void RegistrarMascota_ConSexoInvalido_DebeRechazarse()
    {
        // Comprobar que el sexo cumpla la regla de negocio
        Assert.Throws<ReglaNegocioException>(
            () => new Mascota(
                1, "Firulais", "Perro", "Labrador", "Otro", 5m));
    }

    [Fact]
    public void DesactivarMascota_DebeCambiarSuEstado()
    {
        // Crear una mascota activa
        var mascota = new Mascota(
            1, "Firulais", "Perro", "Labrador", "Macho", 5m);

        // Desactivar la mascota
        mascota.Desactivar();

        // Comprobar que quedó inactiva
        Assert.False(mascota.Activo);
    }
}
