using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectTakeCareBack.Models

{
    public class Psicologo
    {
        public int Id { get; set; }

        public int? IdUsuario { get; set; }

        public string? CedulaProfesional { get; set; } = null!;
        public string? Especialidad { get; set; } = null!;
        public string? Descripcion { get; set; }
        public int? ExperienciaAnios { get; set; }
        public string? UniversidadEgreso { get; set; }
        public string? DireccionConsultorio { get; set; }

        public decimal? CalificacionPromedio { get; set; } = 0;
        public int? TotalResenas { get; set; } = 0;


        public Usuario Usuario { get; set; } = null!;

        [JsonIgnore]
        public ICollection<Cita>? Citas { get; set; }

        [JsonIgnore]
        public ICollection<Recurso>? Recursos { get; set; }

        [JsonIgnore]
        public ICollection<SuscripcionPsicologo>? Suscripciones { get; set; }
    }
}
