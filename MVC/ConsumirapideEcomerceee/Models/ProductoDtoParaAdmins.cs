namespace ConsumirapideEcomerceee.Models
{
    public class ProductoDtoParaAdmins
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string ImagenUrl { get; set; }
        public string CategoriaNombre { get; set; }
        public bool tieneoferta { get; set; }
        public float PrecioOriginal { get; set; }
        public float PrecioFinal { get; set; }
        public int Cantidad { get; set; }
    }
}
