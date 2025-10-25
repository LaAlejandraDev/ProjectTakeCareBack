using Microsoft.EntityFrameworkCore;
using BackTakeCare.Models;
using System.Collections.Generic;

namespace BackTakeCare.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<Psicologo> Psicologos { get; set; }
    public DbSet<DiarioEmocional> DiarioEmocional { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Paciente>()
                .HasOne(p => p.Usuario)
                .WithMany() 
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Psicologo>()
                .HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Paciente>()
                .HasOne(p => p.PsicologoAsignado)
                .WithMany(p => p.Pacientes)
                .HasForeignKey(p => p.PsicologoAsignadoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

