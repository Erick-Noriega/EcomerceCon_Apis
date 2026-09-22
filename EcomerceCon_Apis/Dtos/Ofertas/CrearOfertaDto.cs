namespace EcomerceCon_Apis.Dtos.Ofertas
{
    public class    CrearOfertaDto
    {

      
        public string MotivoDeOferta { get; set; }
        public float MontoAdescontar { get; set; }
        public DateTime Fecha_inicio { get; set; }
        public DateTime Fecha_fin { get; set; }
         public List<int> ProductoIds { get; set; } = new List<int>();
    }
}
