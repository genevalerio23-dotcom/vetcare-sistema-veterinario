
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VetCare.Domain.Entities;
using VetCare.Infrastructure.Data;
using VetCare.Infrastructure.Repositories;
using Xunit;

namespace VetCare.Tests;

public class CitaConcurrenciaTests
{
    private static DbContextOptions<VetCareDbContext> CrearOpciones()
    {
        // Leer la conexión de pruebas
        var conexion = Environment.GetEnvironmentVariable(
            "VETCARE_TEST_CONNECTION");

        if (string.IsNullOrWhiteSpace(conexion))
        {
            throw new InvalidOperationException(
                "Configure VETCARE_TEST_CONNECTION para ejecutar esta prueba.");
        }

        var opciones = new DbContextOptionsBuilder<VetCareDbContext>()
            .UseSqlServer(conexion)
            .Options;

        // Evitar usar accidentalmente la base principal
        using var contexto = new VetCareDbContext(opciones);

        if (!string.Equals(
            contexto.Database.GetDbConnection().Database,
            "VetCareDB_Prueba",
            StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Esta prueba solo puede ejecutarse en VetCareDB_Prueba.");
        }

        return opciones;
    }

    [Fact]
    public async Task DosReservasSimultaneas_NoDebenDuplicarse()
    {
        var opciones = CrearOpciones();

        // Identificar exclusivamente los registros de esta prueba
        var motivo = "TEST-CONC-" + Guid.NewGuid().ToString("N");
        var fecha = DateTime.Today.AddDays(90)
            .AddHours(10)
            .AddMinutes(Random.Shared.Next(0, 480));

        // Ambas operaciones comenzarán desde esta señal
        var inicio = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        async Task<Exception?> IntentarReserva()
        {
            await using var contexto =
                new VetCareDbContext(opciones);

            var repositorio = new CitaRepository(contexto);

            var cita = new Cita(
                1,
                1,
                fecha,
                motivo);

            await inicio.Task;

            try
            {
                await repositorio.AgregarAsync(cita);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        try
        {
            // Crear dos operaciones independientes
            var solicitudA = IntentarReserva();
            var solicitudB = IntentarReserva();

            // Liberar ambas operaciones
            inicio.SetResult(true);

            var resultados = await Task.WhenAll(
                solicitudA, solicitudB);

            // Consultar el estado final de la base de datos
            await using var verificacion =
                new VetCareDbContext(opciones);

            var citasGuardadas = await verificacion.Citas
                .AsNoTracking()
                .Where(c => c.Motivo == motivo)
                .CountAsync();

            var exitosas = resultados.Count(r => r is null);
            var rechazadas = resultados.Count(r => r is not null);

            // No deben quedar reservas duplicadas
            Assert.Equal(1, citasGuardadas);

            // Una debe guardar y otra debe fallar
            Assert.Equal(1, exitosas);
            Assert.Equal(1, rechazadas);
        }
        finally
        {
            // Eliminar solamente los datos de esta ejecución
            await using var limpieza =
                new VetCareDbContext(opciones);

            await limpieza.Citas
                .Where(c => c.Motivo == motivo)
                .ExecuteDeleteAsync();
        }
    }
}

