namespace ConsumirapideEcomerceee.Models
{
    public class CrearoofertaDto
    {
        public string MotivoDeOferta { get; set; }
        public float MontoAdescontar { get; set; }
        public DateTime Fecha_inicio { get; set; }
        public DateTime Fecha_fin { get; set; }
        public List<int> ProductoIds {  get; set; }
    }
}
