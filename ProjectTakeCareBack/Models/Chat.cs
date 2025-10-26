namespace ProjectTakeCareBack.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public int IdPsicologo { get; set; }
        public int IdPaciente { get; set; }
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public DateTime? UltimoMensajeEn { get; set; }

        public Psicologo Psicologo { get; set; } = null!;
        public Paciente Paciente { get; set; } = null!;
        public ICollection<ChatMensaje>? Mensajes { get; set; }
    }
}
