namespace ProjectTakeCareBack.Models
{
    public class CrearCitaDto
    {
        public int IdPsicologo { get; set; }
        public int IdPaciente { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public string? Motivo { get; set; }
        public string? Ubicacion { get; set; }
    }
}