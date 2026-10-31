namespace ConsumirapideEcomerceee.Models
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; }
        public float PrecioOriginal { get; set; }
        public float Preciofinal { get; set; }
        public bool tieneoferta { get; set; }
        public float PorcentajeDeDescuento { get; set; }
    }
}
