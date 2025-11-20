using System.Text.Json.Serialization;

namespace ProjectTakeCareBack.Models
{
    public class PsicologoDisponibilidad
    {
        public int Id { get; set; }

        public int IdPsicologo { get; set; }

        public DateTime Fecha { get; set; }

        public int CupoMaximo { get; set; } = 4;

        public int CitasAgendadas { get; set; } = 0;

        [JsonIgnore]
        public Psicologo? Psicologo { get; set; } = null!;
    }
}
