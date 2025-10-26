namespace ProjectTakeCareBack.Models
{
    public class Archivo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string TipoMime { get; set; } = null!;
        public long Tamano { get; set; }
        public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
    }
}
