namespace ProjectTakeCareBack.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int IdPost { get; set; }
        public int IdUsuario { get; set; }
        public string Contenido { get; set; } = null!;
        public int Likes { get; set; } = 0;
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public bool Anonimo { get; set; } = false;

        public Post Post { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
    }
}
