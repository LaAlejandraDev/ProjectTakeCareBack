using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectTakeCareBack.Models

{
    public class Paciente
    {
        public int Id { get; set; }

        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public DateOnly FechaNacimiento { get; set; }
        public string Genero { get; set; } = null!;
        public string? Ciudad { get; set; }
        public string? EstadoCivil { get; set; }

        public string? Diagnostico { get; set; }
        public string? AntecedentesMedicos { get; set; }
        public string? ContactoEmergencia { get; set; }

        public ICollection<Cita>? Citas { get; set; }
        public ICollection<Chat>? Chats { get; set; }
        public ICollection<DiarioEmocional>? DiarioEmocional { get; set; }
        public ICollection<CrisisAlerta>? CrisisAlerts { get; set; }
    }
}
