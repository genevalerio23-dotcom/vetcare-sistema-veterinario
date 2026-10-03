using Microsoft.EntityFrameworkCore;
using VetCare.Domain.Entities;

namespace VetCare.Infrastructure.Data;

public class VetCareDbContext : DbContext
{
    public VetCareDbContext(
        DbContextOptions<VetCareDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Mascota> Mascotas => Set<Mascota>();
    public DbSet<Veterinario> Veterinarios => Set<Veterinario>();
    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<Consulta> Consultas => Set<Consulta>();
    public DbSet<Tratamiento> Tratamientos => Set<Tratamiento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Nombres)
                .HasMaxLength(100).IsRequired();

            entity.Property(c => c.Apellidos)
                .HasMaxLength(100).IsRequired();

            entity.Property(c => c.Telefono)
                .HasMaxLength(20).IsRequired();

            entity.Property(c => c.Email)
                .HasMaxLength(150).IsRequired();

            entity.Property(c => c.Direccion)
                .HasMaxLength(250).IsRequired();
        });

        modelBuilder.Entity<Mascota>(entity =>
        {
            entity.ToTable("Mascotas", table =>
            {
                table.HasCheckConstraint(
                    "CK_Mascotas_Peso",
                    "[Peso] > 0");

                table.HasCheckConstraint(
                    "CK_Mascotas_Sexo",
                    "[Sexo] IN (N'Macho', N'Hembra')");
            });

            entity.HasKey(m => m.Id);

            entity.Property(m => m.Nombre)
                .HasMaxLength(100).IsRequired();

            entity.Property(m => m.Especie)
                .HasMaxLength(50).IsRequired();

            entity.Property(m => m.Raza)
                .HasMaxLength(100).IsRequired();

            entity.Property(m => m.Sexo)
                .HasMaxLength(10).IsRequired();

            entity.Property(m => m.Peso)
                .HasPrecision(6, 2);

            entity.HasOne<Cliente>()
                .WithMany()
                .HasForeignKey(m => m.ClienteId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Veterinario>(entity =>
        {
            entity.ToTable("Veterinarios");
            entity.HasKey(v => v.Id);

            entity.Property(v => v.Nombre)
                .HasMaxLength(150).IsRequired();

            entity.Property(v => v.Especialidad)
                .HasMaxLength(100).IsRequired();

            entity.Property(v => v.Telefono)
                .HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<Cita>(entity =>
        {
            entity.ToTable("Citas", table =>
            {
                table.HasCheckConstraint(
                    "CK_Citas_Estado",
                    "[Estado] IN (N'Programada', N'Atendida', N'Cancelada')");
            });

            entity.HasKey(c => c.Id);

            entity.Property(c => c.FechaHora)
                .HasColumnType("datetime2");

            entity.Property(c => c.Motivo)
                .HasMaxLength(250).IsRequired();

            entity.Property(c => c.Estado)
                .HasMaxLength(20).IsRequired();

            entity.Ignore(c => c.FechaHoraFin);

            entity.HasOne<Mascota>()
                .WithMany()
                .HasForeignKey(c => c.MascotaId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Veterinario>()
                .WithMany()
                .HasForeignKey(c => c.VeterinarioId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(c => new
            {
                c.VeterinarioId,
                c.FechaHora
            });

            entity.HasIndex(c => new
            {
                c.MascotaId,
                c.FechaHora
            });
        });

        modelBuilder.Entity<Consulta>(entity =>
        {
            entity.ToTable("Consultas");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Fecha)
                .HasColumnType("datetime2");

            entity.Property(c => c.Motivo)
                .HasMaxLength(250).IsRequired();

            entity.Property(c => c.Diagnostico)
                .HasMaxLength(2000).IsRequired();

            entity.Property(c => c.Observaciones)
                .HasMaxLength(2000).IsRequired();

            entity.HasOne<Mascota>()
                .WithMany()
                .HasForeignKey(c => c.MascotaId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Veterinario>()
                .WithMany()
                .HasForeignKey(c => c.VeterinarioId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne<Cita>()
                .WithMany()
                .HasForeignKey(c => c.CitaId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(c => c.CitaId)
                .IsUnique()
                .HasFilter("[CitaId] IS NOT NULL");

            entity.HasIndex(c => new
            {
                c.MascotaId,
                c.Fecha
            });
        });

        modelBuilder.Entity<Tratamiento>(entity =>
        {
            entity.ToTable("Tratamientos", table =>
            {
                table.HasCheckConstraint(
                    "CK_Tratamientos_Fechas",
                    "[FechaFin] IS NULL OR [FechaFin] >= [FechaInicio]");
            });

            entity.HasKey(t => t.Id);

            entity.Property(t => t.Descripcion)
                .HasMaxLength(500).IsRequired();

            entity.Property(t => t.Indicaciones)
                .HasMaxLength(2000).IsRequired();

            entity.Property(t => t.FechaInicio)
                .HasColumnType("date");

            entity.Property(t => t.FechaFin)
                .HasColumnType("date");

            entity.HasOne<Consulta>()
                .WithMany()
                .HasForeignKey(t => t.ConsultaId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}