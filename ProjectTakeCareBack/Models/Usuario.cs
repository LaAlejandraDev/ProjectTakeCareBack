using Microsoft.Extensions.Hosting;
using ProjectTakeCareBack.Enums;
using System.Text.Json.Serialization;

namespace ProjectTakeCareBack.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;
        public string ApellidoPaterno { get; set; } = null!;
        public string? ApellidoMaterno { get; set; }
        public string Correo { get; set; } = null!;
        public string Telefono { get; set; } = null!;

        public string Contrasena { get; set; } = null!;

        public RolUsuario Rol { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public DateTime? UltimoAcceso { get; set; }

        [JsonIgnore]
        public Psicologo? Psicologo { get; set; }

        [JsonIgnore]

        public Paciente? Paciente { get; set; }

        [JsonIgnore]
        public ICollection<Post>? Publicaciones { get; set; }

        [JsonIgnore]
        public ICollection<Comentario>? Comentarios { get; set; }

        [JsonIgnore]
        public ICollection<Notificacion>? Notificaciones { get; set; }
    }
}
