namespace BackTakeCare.Models
{
    public class DiarioEmocional
    {

        /* HOLAAA */
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; } = null!;
        public DateTime Fecha { get; set; } = DateTime.Today;
        public int NivelAnimo { get; set; }
        public string? PalabrasClave { get; set; }
        public string? Narrativa { get; set; }
        public double RiesgoEmocional { get; set; }
        public bool AlertaEnviada { get; set; } = false;

    }
}
