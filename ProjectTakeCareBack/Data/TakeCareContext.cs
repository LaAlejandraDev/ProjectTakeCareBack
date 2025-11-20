using Microsoft.EntityFrameworkCore;
using ProjectTakeCareBack.Models;
using ProjectTakeCareBack.Enums;

namespace ProjectTakeCareBack.Data
{
    public class TakeCareContext : DbContext
    {
        public TakeCareContext(DbContextOptions<TakeCareContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Psicologo> Psicologos { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMensaje> ChatMensajes { get; set; }
        public DbSet<Recurso> Recursos { get; set; }
        public DbSet<Archivo> Archivos { get; set; }
        public DbSet<DiarioEmocional> DiarioEmocional { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<CrisisAlerta> CrisisAlerts { get; set; }
        public DbSet<Plan> Planes { get; set; }
        public DbSet<SuscripcionPsicologo> Suscripciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().Property(u => u.Rol).HasConversion<string>();
            modelBuilder.Entity<Post>().Property(p => p.Tipo).HasConversion<string>();
            modelBuilder.Entity<CrisisAlerta>().Property(a => a.Severidad).HasConversion<string>();

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Psicologo)
                .WithOne(p => p.Usuario)
                .HasForeignKey<Psicologo>(p => p.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Paciente)
                .WithOne(p => p.Usuario)
                .HasForeignKey<Paciente>(p => p.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Publicaciones)
                .HasForeignKey(p => p.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comentario>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comentarios)
                .HasForeignKey(c => c.IdPost)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comentario>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Comentarios)
                .HasForeignKey(c => c.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Psicologo>()
                .HasMany(p => p.Citas)
                .WithOne(c => c.Psicologo)
                .HasForeignKey(c => c.IdPsicologo)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Psicologo>()
                .HasMany(p => p.Recursos)
                .WithOne(r => r.Psicologo)
                .HasForeignKey(r => r.IdPsicologo)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Psicologo>()
                .HasMany(p => p.Suscripciones)
                .WithOne(s => s.Psicologo)
                .HasForeignKey(s => s.IdPsicologo)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Paciente>()
                .HasMany(p => p.Citas)
                .WithOne(c => c.Paciente)
                .HasForeignKey(c => c.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Paciente>()
                .HasMany(p => p.Chats)
                .WithOne(c => c.Paciente)
                .HasForeignKey(c => c.IdPaciente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Paciente>()
                .HasMany(p => p.DiarioEmocional)
                .WithOne(d => d.Paciente)
                .HasForeignKey(d => d.IdPaciente)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Chat>()
                .HasMany(c => c.Mensajes)
                .WithOne(m => m.Chat)
                .HasForeignKey(m => m.IdChat)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CrisisAlerta>()
                .HasOne(c => c.Paciente)
                .WithMany(p => p.CrisisAlerts)
                .HasForeignKey(c => c.IdPaciente)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notificacion>()
                .HasOne(n => n.Usuario)
                .WithMany(u => u.Notificaciones)
                .HasForeignKey(n => n.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SuscripcionPsicologo>()
                .HasOne(s => s.Plan)
                .WithMany()
                .HasForeignKey(s => s.IdPlan)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Plan>()
                .Property(p => p.Precio)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Psicologo>()
                .Property(p => p.CalificacionPromedio)
                .HasPrecision(3, 2);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo)
                .IsUnique();

            modelBuilder.Entity<Cita>()
                .HasCheckConstraint("CK_Cita_Fechas", "[FechaInicio] < [FechaFin]");
        }
        public DbSet<ProjectTakeCareBack.Models.PsicologoDisponibilidad> PsicologoDisponibilidad { get; set; } = default!;
    }
}
