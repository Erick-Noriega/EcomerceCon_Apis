namespace ConsumirapideEcomerceee.Models
{
    public class EditarProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int cantidad { get; set; }
        public float precio { get; set; }

        public string ImagenUrl { get; set; }
        public int CategoriaId { get; set; }




    }
}
