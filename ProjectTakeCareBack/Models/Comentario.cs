using System.ComponentModel.DataAnnotations;

namespace ProjectTakeCareBack.Models
{
    public class Comentario
    {
        public int Id { get; set; }

        [Required]
        public int IdPost { get; set; }
        public Post Post { get; set; } = null!;

        [Required]
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;

        [Required(ErrorMessage = "El contenido del comentario es obligatorio")]
        [MinLength(25, ErrorMessage = "El comentario debe tener al menos 25 caracteres")]
        public string Contenido { get; set; } = null!;

        public int Likes { get; set; } = 0;
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public bool Anonimo { get; set; } = false;
    }
}
