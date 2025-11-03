using System.Text.Json.Serialization;

namespace ProjectTakeCareBack.Models
{
    public class Notificacion
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string Titulo { get; set; } = null!;
        public string Mensaje { get; set; } = null!;
        public bool Leida { get; set; } = false;
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public Usuario? Usuario { get; set; } = null!;
    }
}
