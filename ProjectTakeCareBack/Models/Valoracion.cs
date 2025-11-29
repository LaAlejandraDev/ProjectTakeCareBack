using System.Text.Json.Serialization;

namespace ProjectTakeCareBack.Models
{
    public class Valoracion
    {
        public int Id { get; set; }
        public float Calificacion { get; set; }
        public int IdPsicologo { get; set; }
        public int IdCita { get; set; }
        [JsonIgnore]
        public Cita? Cita { get; set; }
        [JsonIgnore]
        public Psicologo? Psicologo { get; set; }
    }
}
