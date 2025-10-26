namespace ProjectTakeCareBack.Models
{
    public class Cita
    {
        public int Id { get; set; }
        public int IdPsicologo { get; set; }
        public int IdPaciente { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } = "Programada";
        public string? Motivo { get; set; }
        public string? Ubicacion { get; set; }

        public Psicologo Psicologo { get; set; } = null!;
        public Paciente Paciente { get; set; } = null!;
    }
}
