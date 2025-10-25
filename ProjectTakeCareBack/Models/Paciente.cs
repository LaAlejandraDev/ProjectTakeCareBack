namespace BackTakeCare.Models
{
    public class Paciente
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public DateOnly FechaNacimiento { get; set; }
        public string Genero { get; set; } = string.Empty;
        public string? Diagnostico { get; set; }
        public string? Direccion { get; set; }
        public bool Activo { get; set; } = true;
        public ICollection<DiarioEmocional>? DiarioEmocional { get; set; }
        public int PsicologoAsignadoId { get; set; }
        public Psicologo PsicologoAsignado { get; set; } = null!;
    }
}
