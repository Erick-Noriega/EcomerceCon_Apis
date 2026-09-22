using System.Reflection.Metadata.Ecma335;

namespace EcomerceCon_Apis.Dtos.Ofertas
{
    public class OfertaDto
    {
        public int Id { get; set; }
        public string MotivoDeOferta { get; set; }
        public float porcentajeADescontar { get; set; }
        public DateTime Fecha_inicio { get; set; }
        public DateTime Fecha_fin { get; set; }
        public bool disponible { get; set; }
        public List<int> IdProductos { get; set; }=new List<int>();


      }


    }
