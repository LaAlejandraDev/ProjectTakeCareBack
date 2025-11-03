using System.Text.Json.Serialization;

namespace ProjectTakeCareBack.Models
{
    public class SuscripcionPsicologo
    {
        public int Id { get; set; }
        public int IdPsicologo { get; set; }
        public int IdPlan { get; set; }
        public DateTime FechaInicio { get; set; } = DateTime.UtcNow;
        public DateTime? FechaFin { get; set; }
        public string Estado { get; set; } = "Activa";

        [JsonIgnore]
        public Psicologo? Psicologo { get; set; } = null!;

        [JsonIgnore]    
        public Plan? Plan { get; set; } = null!;
    }
}
