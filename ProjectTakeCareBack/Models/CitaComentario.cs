namespace ProjectTakeCareBack.Models
{
    public class CitaComentario
    {
        public int Id { get; set; }
        public int CitaId { get; set; }
        public Cita? Cita { get; set; }

        public string? Comentario { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
