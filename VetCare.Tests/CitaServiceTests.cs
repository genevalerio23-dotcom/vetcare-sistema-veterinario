
using System;
using System.Threading.Tasks;
using Moq;
using VetCare.Application.DTOs;
using VetCare.Application.Services;
using VetCare.Domain.Entities;
using VetCare.Domain.Exceptions;
using VetCare.Domain.Interfaces;
using Xunit;

namespace VetCare.Tests;

public class CitaServiceTests
{
    private static CrearCitaDto DatosValidos()
    {
        return new CrearCitaDto
        {
            MascotaId = 1,
            VeterinarioId = 1,
            FechaHora = DateTime.Now.AddDays(2),
            Motivo = "Consulta general"
        };
    }

    private static (
        CitaService Servicio,
        Mock<ICitaRepository> Citas,
        Mock<IMascotaRepository> Mascotas,
        Mock<IVeterinarioRepository> Veterinarios)
        PrepararServicio()
    {
        // Crear los repositorios simulados
        var citas = new Mock<ICitaRepository>();
        var mascotas = new Mock<IMascotaRepository>();
        var veterinarios = new Mock<IVeterinarioRepository>();

        // Preparar una mascota activa
        var mascota = new Mascota(
            1, "Firulais", "Perro", "Labrador",
            "Macho", 5m);

        // Preparar un veterinario activo
        var veterinario = new Veterinario(
            "Carlos López", "Medicina general",
            "88888888");

        mascotas
            .Setup(x => x.ObtenerPorIdAsync(1))
            .ReturnsAsync(mascota);

        veterinarios
            .Setup(x => x.ObtenerPorIdAsync(1))
            .ReturnsAsync(veterinario);

        // Inicialmente no existen conflictos
        citas
            .Setup(x => x.ExisteConflictoAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .ReturnsAsync(false);

        citas
            .Setup(x => x.AgregarAsync(It.IsAny<Cita>()))
            .Returns(Task.CompletedTask);

        var servicio = new CitaService(
            citas.Object,
            mascotas.Object,
            veterinarios.Object,
            TimeProvider.System);

        return (servicio, citas, mascotas, veterinarios);
    }

    [Fact]
    public async Task Agendar_EnFechaPasada_DebeRechazarse()
    {
        var prueba = PrepararServicio();
        var dto = DatosValidos();
        dto.FechaHora = DateTime.Now.AddDays(-1);

        await Assert.ThrowsAsync<ReglaNegocioException>(
            () => prueba.Servicio.AgendarAsync(dto));

        prueba.Citas.Verify(
            x => x.AgregarAsync(It.IsAny<Cita>()),
            Times.Never);
    }

    [Fact]
    public async Task Agendar_MascotaInexistente_DebeRechazarse()
    {
        var prueba = PrepararServicio();

        prueba.Mascotas
            .Setup(x => x.ObtenerPorIdAsync(1))
            .ReturnsAsync((Mascota?)null);

        await Assert.ThrowsAsync<ReglaNegocioException>(
            () => prueba.Servicio.AgendarAsync(DatosValidos()));

        prueba.Citas.Verify(
            x => x.AgregarAsync(It.IsAny<Cita>()),
            Times.Never);
    }

    [Fact]
    public async Task Agendar_MascotaInactiva_DebeRechazarse()
    {
        var prueba = PrepararServicio();

        var mascota = new Mascota(
            1, "Firulais", "Perro", "Labrador",
            "Macho", 5m);

        mascota.Desactivar();

        prueba.Mascotas
            .Setup(x => x.ObtenerPorIdAsync(1))
            .ReturnsAsync(mascota);

        await Assert.ThrowsAsync<ReglaNegocioException>(
            () => prueba.Servicio.AgendarAsync(DatosValidos()));
    }

    [Fact]
    public async Task Agendar_VeterinarioInexistente_DebeRechazarse()
    {
        var prueba = PrepararServicio();

        prueba.Veterinarios
            .Setup(x => x.ObtenerPorIdAsync(1))
            .ReturnsAsync((Veterinario?)null);

        await Assert.ThrowsAsync<ReglaNegocioException>(
            () => prueba.Servicio.AgendarAsync(DatosValidos()));
    }

    [Fact]
    public async Task Agendar_VeterinarioInactivo_DebeRechazarse()
    {
        var prueba = PrepararServicio();

        var veterinario = new Veterinario(
            "Carlos López", "Medicina general",
            "88888888");

        veterinario.Desactivar();

        prueba.Veterinarios
            .Setup(x => x.ObtenerPorIdAsync(1))
            .ReturnsAsync(veterinario);

        await Assert.ThrowsAsync<ReglaNegocioException>(
            () => prueba.Servicio.AgendarAsync(DatosValidos()));
    }

    [Fact]
    public async Task Agendar_ConHorarioOcupado_DebeRechazarse()
    {
        var prueba = PrepararServicio();

        // Simular un conflicto de horario
        prueba.Citas
            .Setup(x => x.ExisteConflictoAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<ReglaNegocioException>(
            () => prueba.Servicio.AgendarAsync(DatosValidos()));

        // No debe intentar guardar la cita
        prueba.Citas.Verify(
            x => x.AgregarAsync(It.IsAny<Cita>()),
            Times.Never);
    }

    [Fact]
    public async Task Agendar_ConDatosValidos_DebeGuardar()
    {
        var prueba = PrepararServicio();

        await prueba.Servicio.AgendarAsync(DatosValidos());

        // Debe solicitar el registro una sola vez
        prueba.Citas.Verify(
            x => x.AgregarAsync(It.IsAny<Cita>()),
            Times.Once);
    }
}

