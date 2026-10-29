
using System;
using VetCare.Domain.Entities;
using VetCare.Domain.Exceptions;
using Xunit;

namespace VetCare.Tests;

public class CitaTests
{
    [Fact]
    public void CrearCita_ConDatosValidos_DebeCrearse()
    {
        // Crear una cita válida
        var fecha = new DateTime(2026, 12, 15, 10, 0, 0);

        var cita = new Cita(
            1, 1, fecha, "Consulta general");

        // Verificar sus datos
        Assert.Equal(1, cita.MascotaId);
        Assert.Equal(1, cita.VeterinarioId);
        Assert.Equal(fecha, cita.FechaHora);
        Assert.Equal("Programada", cita.Estado);
    }

    [Fact]
    public void CrearCita_DebeDurar30Minutos()
    {
        // Comprobar la duración de la cita
        var fecha = new DateTime(2026, 12, 15, 10, 0, 0);

        var cita = new Cita(
            1, 1, fecha, "Vacunación");

        Assert.Equal(
            fecha.AddMinutes(30),
            cita.FechaHoraFin);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CrearCita_ConMascotaInvalida_DebeRechazarse(
        int mascotaId)
    {
        // Rechazar identificadores inválidos
        Assert.Throws<ReglaNegocioException>(() =>
            new Cita(
                mascotaId,
                1,
                new DateTime(2026, 12, 15),
                "Consulta"));
    }

    [Fact]
    public void CrearCita_SinVeterinario_DebeRechazarse()
    {
        // Validar que se seleccione veterinario
        Assert.Throws<ReglaNegocioException>(() =>
            new Cita(
                1, 0,
                new DateTime(2026, 12, 15),
                "Consulta"));
    }

    [Fact]
    public void CrearCita_SinMotivo_DebeRechazarse()
    {
        // Validar que el motivo sea obligatorio
        Assert.Throws<ReglaNegocioException>(() =>
            new Cita(
                1, 1,
                new DateTime(2026, 12, 15),
                ""));
    }

    [Fact]
    public void MarcarAtendida_DebeCambiarEstado()
    {
        // Cambiar una cita programada a atendida
        var cita = new Cita(
            1, 1,
            new DateTime(2026, 12, 15),
            "Consulta");

        cita.MarcarAtendida();

        Assert.Equal("Atendida", cita.Estado);
    }

    [Fact]
    public void CancelarCita_DebeCambiarEstado()
    {
        // Cancelar una cita programada
        var cita = new Cita(
            1, 1,
            new DateTime(2026, 12, 15),
            "Consulta");

        cita.Cancelar();

        Assert.Equal("Cancelada", cita.Estado);
    }

    [Fact]
    public void CancelarCitaAtendida_DebeRechazarse()
    {
        // No permitir cancelar una cita atendida
        var cita = new Cita(
            1, 1,
            new DateTime(2026, 12, 15),
            "Consulta");

        cita.MarcarAtendida();

        Assert.Throws<ReglaNegocioException>(
            () => cita.Cancelar());
    }
}

