using System.Text.Json.Serialization;

namespace ProjectTakeCareBack.Models
{
    public class Recurso
    {
        public int Id { get; set; }
        public int IdPsicologo { get; set; }
        public string Titulo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string Tipo { get; set; } = "PDF";
        public string Url { get; set; } = null!;
        public DateTime FechaSubida { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public Psicologo? Psicologo { get; set; } = null!;
    }
}
