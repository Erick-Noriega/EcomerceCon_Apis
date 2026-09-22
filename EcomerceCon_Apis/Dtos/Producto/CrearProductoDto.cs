namespace EcomerceCon_Apis.Dtos.Producto
{
    public class CrearProductoDto
    {
      
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int cantidad { get; set; }
        public float precio { get; set; }

        public string  ImagenUrl { get; set; }
        public int CategoriaId { get; set; }
     


    }
}
