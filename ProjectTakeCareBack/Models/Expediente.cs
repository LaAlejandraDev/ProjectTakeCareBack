using System.Text.Json.Serialization;

namespace ProjectTakeCareBack.Models
{
    public class Expediente
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public Paciente? Paciente { get; set; }
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
        public ICollection<DiarioEmocional> Diarios { get; set; } = new List<DiarioEmocional>();
    }
}
