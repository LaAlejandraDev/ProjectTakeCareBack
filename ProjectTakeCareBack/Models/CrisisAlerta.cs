using ProjectTakeCareBack.Enums;

namespace ProjectTakeCareBack.Models
{
    public class CrisisAlerta
    {
        public int Id { get; set; }
        public int IdPaciente { get; set; }
        public SeveridadAlerta Severidad { get; set; } = SeveridadAlerta.Baja;
        public string[]? TerminosDetectados { get; set; }
        public bool Atendido { get; set; } = false;
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public Paciente Paciente { get; set; } = null!;
    }
}
