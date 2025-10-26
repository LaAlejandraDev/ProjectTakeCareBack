namespace ProjectTakeCareBack.Models
{
    public class ChatMensaje
    {
        public int Id { get; set; }
        public int IdChat { get; set; }
        public int IdRemitenteUsuario { get; set; }
        public string Mensaje { get; set; } = null!;
        public bool Leido { get; set; } = false;
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public Chat Chat { get; set; } = null!;
    }
}
