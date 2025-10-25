namespace BackTakeCare.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public string ApellidoPaterno { get; set; } = null!;

        public string ApellidoMaterno { get; set; } = null!;

        public string Telefono { get; set; } = null!;

        public DateOnly FechaNacimiento { get; set; }

        public string Pais { get; set; } = null!;

        public string Correo { get; set; } = null!;

        public string? Contrasena { get; set; } = null!;
        public string Rol { get; set; } = "Paciente";

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public DateTime? UltimoAcceso { get; set; }

        public bool Activo { get; set; } = true;


        /*ahsfsdbd*/
    }
}
