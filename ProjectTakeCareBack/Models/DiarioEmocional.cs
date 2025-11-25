using System.Text.Json.Serialization;

namespace ProjectTakeCareBack.Models
{
    public class DiarioEmocional
    {
        public int Id { get; set; }
        public int IdPaciente { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public string EstadoEmocional { get; set; } = null!;
        public int? Nivel { get; set; }
        public string? Comentario { get; set; }

        [JsonIgnore]
        public Paciente? Paciente { get; set; } = null!;
    }
}
