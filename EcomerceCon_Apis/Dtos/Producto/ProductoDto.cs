namespace EcomerceCon_Apis.Dtos.Producto
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string ImagenUrl { get; set; }
        public string Descripcion { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; }
        public float PrecioOriginal { get; set; }
        public float PrecioFinal { get; set; }
        public bool TieneOferta {  get; set; }

    }
}
