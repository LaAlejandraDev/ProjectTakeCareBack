namespace ProjectTakeCareBack.Models
{
    public class PostResponse
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string Contenido { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public int Tipo { get; set; }
        public int IdUsuario { get; set; }
        public UsuarioResponse Usuario { get; set; } = null!;
        public bool Anonimo { get; set; }
        public int LikesCount { get; set; }
        public int CommentCount { get; set; }
    }

    public class UsuarioResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
    }

}
