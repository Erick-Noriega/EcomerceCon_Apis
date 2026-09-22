namespace EcomerceCon_Apis.Models
{
    public class DetalleCarrito
    {
        public int Id { get; set; }
        public Producto? producto { get; set; }
        public int ProductoId { get; set; }
        public int CarritoId { get; set; }
        public Carrito? carrito { get; set; }
        public int cantidad { get; set; }
        public float PrecioPorUnidad { get; set; }


    }
}
