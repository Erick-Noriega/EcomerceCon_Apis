namespace EcomerceCon_Apis.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public float precio { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int cantidad { get; set; }

        public string  ImagenUrl { get; set; }
        public int CategoriaId { get; set; }

        public Categoria? categoria { get; set; }
        public List<DetalleCarrito>? DetalleCarrito { get; set; }
        public List<DetallePedido>? detallePedido { get; set; }
        public List<OfertaProducto>? ofertas { get; set; }



    }
}
