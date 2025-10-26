namespace ProjectTakeCareBack.Models
{
    public class Plan
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public decimal Precio { get; set; }
        public int DuracionDias { get; set; }
        public string? Caracteristicas { get; set; }
    }
}
