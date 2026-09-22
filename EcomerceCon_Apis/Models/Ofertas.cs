namespace EcomerceCon_Apis.Models
{
    public class Ofertas
    {
        public int Id { get; set; }
        public string  MotivoDeOferta { get; set; }
        public float  MontoAdescontar { get; set; }
        public DateTime Fecha_inicio { get; set; }
        public DateTime Fecha_fin { get; set; }
        public bool disponible { get; set; }
        public List<OfertaProducto>? Productos { get; set; }

    }
}
