namespace ConsumirapideEcomerceee.Models
{
    public class CrearProductoDto
    {
        public string Nombre { get; set; }
        public string  descripcion { get; set; }
        public int Cantidad { get; set; }
        public float Precio { get; set; }
        public string ImagenUrl { get; set; }

        public int CategoriaId { get; set; }


    }
}
