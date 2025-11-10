using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectTakeCareBack.Models
{
    public class Chat   
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Psicologo))]
        public int IdPsicologo { get; set; }
        [ForeignKey(nameof(Paciente))]
        public int IdPaciente { get; set; }
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public DateTime? UltimoMensajeEn { get; set; }
        public bool Activo = true;

        [JsonIgnore]
        public Psicologo? Psicologo { get; set; } = null!;

        [JsonIgnore] 
        public Paciente? Paciente { get; set; } = null!;

        [JsonIgnore] 
        public ICollection<ChatMensaje>? Mensajes { get; set; }
    }
}
