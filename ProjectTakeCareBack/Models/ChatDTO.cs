namespace ProjectTakeCareBack.Models
{
    public class ChatDTO
    {
        public int Id { get; set; }
        public int IdPsicologo { get; set; }
        public int IdPaciente { get; set; }
        public DateTime CreadoEn { get; set; }
        public DateTime? UltimoMensajeEn { get; set; }

        public string? NombrePsicologo { get; set; }
        public string? Especialidad { get; set; }
        public string? UniversidadEgreso { get; set; }

        public string? NombrePaciente { get; set; }
        public string? ApellidosPaciente { get; set; }
    }
}
