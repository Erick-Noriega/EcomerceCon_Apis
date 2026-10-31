namespace ConsumirapideEcomerceee.Models
{
    public class OfertaDto
    {
        public int Id { get; set; }
        public string MotivoDeOferta { get; set; }
        public float PorcentajeADescontar { get; set; }
        public DateTime Fecha_inicio { get; set; }
        public DateTime Fecha_fin { get; set; }
        public bool  disponible { get; set; }   
    }
}
