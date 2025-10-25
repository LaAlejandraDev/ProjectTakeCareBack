namespace BackTakeCare.Models
{
    public class Psicologo
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public string Especialidad { get; set; } = string.Empty;

        public string? CedulaProfesional { get; set; }
        public string? Matricula { get; set; }

        public string? ReferenciaProfesional { get; set; }


        public bool Activo { get; set; } = true;
        public ICollection<Paciente>? Pacientes { get; set; }
    }
}
