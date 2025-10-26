using ProjectTakeCareBack.Enums;

namespace ProjectTakeCareBack.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string Contenido { get; set; } = null!;
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public PostType Tipo { get; set; }
        public int IdUsuario { get; set; }
        public bool Anonimo { get; set; } = false;

        public int LikesCount { get; set; } = 0;
        public int CommentCount { get; set; } = 0;

        public Usuario Usuario { get; set; } = null!;
        public ICollection<Comment>? Comentarios { get; set; }
    }
}
