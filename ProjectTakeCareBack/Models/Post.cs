using ProjectTakeCareBack.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProjectTakeCareBack.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [MinLength(25, ErrorMessage = "El título debe tener al menos 25 caracteres")]
        public string Titulo { get; set; } = null!;

        [Required(ErrorMessage = "El contenido es obligatorio")]
        [MinLength(50, ErrorMessage = "El contenido debe tener al menos 50 caracteres")]
        public string Contenido { get; set; } = null!;

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Required]
        public PostType Tipo { get; set; }

        [Required]
        public int IdUsuario { get; set; }
        public Usuario? Usuario { get; set; }

        public bool Anonimo { get; set; } = false;

        public int LikesCount { get; set; } = 0;
        public int CommentCount { get; set; } = 0;

        [JsonIgnore]
        public ICollection<Comentario>? Comentarios { get; set; }
    }

}
