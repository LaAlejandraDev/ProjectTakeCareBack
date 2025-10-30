using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectTakeCareBack.Models

{
    public class Paciente
    {
        public int Id { get; set; }

        public int? IdUsuario { get; set; }

        public string? Ciudad { get; set; }
        public string? EstadoCivil { get; set; }

        public string? Diagnostico { get; set; }
        public string? AntecedentesMedicos { get; set; }
        public string? ContactoEmergencia { get; set; }

        public Usuario? Usuario { get; set; } = null!;

        [JsonIgnore]
        public ICollection<Cita>? Citas { get; set; }

        [JsonIgnore]
        public ICollection<Chat>? Chats { get; set; }

        [JsonIgnore]
        public ICollection<DiarioEmocional>? DiarioEmocional { get; set; }

        [JsonIgnore]
        public ICollection<CrisisAlerta>? CrisisAlerts { get; set; }
    }
}
