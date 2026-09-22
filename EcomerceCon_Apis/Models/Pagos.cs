using System.ComponentModel.DataAnnotations.Schema;

namespace EcomerceCon_Apis.Models
{
    public class Pagos
    {
        public int Id { get; set; }
        [ForeignKey(nameof(PedidoId))]
        public Pedido? Pedido { get; set; }
        public int PedidoId { get; set; }

        public float Monto { get; set; }
        public string MetodoPago { get; set; }=string.Empty;
        public string EstadoPago { get; set; } = string.Empty;

        public string TransaccionId { get; set; }=string.Empty;
        public DateTime FechaPago { get; set; }



    }
}
