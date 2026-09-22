namespace EcomerceCon_Apis.Models
{
    public class OfertaProducto
    {
        public int OfertaId { get; set; }
        public int ProductoId { get; set; }
        public Ofertas oferta {  get; set; }
        public Producto producto { get; set; }

    }
}
