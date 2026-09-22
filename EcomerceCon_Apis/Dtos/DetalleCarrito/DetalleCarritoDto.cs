namespace EcomerceCon_Apis.Dtos.DetalleCarrito
{
    public class DetalleCarritoDto
    {
     
        public int Id { get; set; }   
        public int ProductoId { get; set; }    
        public string NombreProducto { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty;
        public float PrecioUnitario { get; set; }
        public int Cantidad { get; set; }

    }
}
