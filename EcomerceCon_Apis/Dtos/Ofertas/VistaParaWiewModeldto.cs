namespace EcomerceCon_Apis.Dtos.Ofertas
{
    public class VistaParaWiewModeldto
    {
        public int OfertaId { get; set; }

        public List<ProductoOfertaDto> ProductosDeOferta { get; set; } = new();

        public List<ProductoOfertaDto> TodosLosProductos { get; set; } = new();
    }
}
