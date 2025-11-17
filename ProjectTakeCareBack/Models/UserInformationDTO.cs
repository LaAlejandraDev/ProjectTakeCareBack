using ProjectTakeCareBack.Enums;

namespace ProjectTakeCareBack.Models
{
    public class UserInformationDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; } = null!;
        public string? ApellidoPaterno { get; set; } = null!;
        public string? ApellidoMaterno { get; set; }
        public string? Genero { get; set; } = null!;
        public string? Correo { get; set; } = null!;
        public string? Telefono { get; set; } = null!;
        public RolUsuario Rol { get; set; }
        public Psicologo? Psicologo { get; set; } // Regresar informacion dependiendo si encuentra coincidencias con el rol
        public Paciente? Paciente { get; set; }
    }
}
