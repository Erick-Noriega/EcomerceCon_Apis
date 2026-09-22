using System.ComponentModel.DataAnnotations.Schema;

namespace EcomerceCon_Apis.Models
{
    public enum EstadoPedido
    {
        Pendiente = 1,
        EnPreparacion = 2,
        Enviado = 3,
        Entregado = 4,
        Cancelado = 5
    }
    public class Pedido
    {
        public int Id { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        public Usuario? Usuario { get; set; }
        public string UsuarioId { get; set; }
        public string  DireccionDeEnvio { get; set; }
        public DateTime fecha_pedido { get; set; }
        public EstadoPedido estadoPedido { get; set; } = EstadoPedido.Pendiente;
        public float  total { get; set; }
        public List<DetallePedido>? detallePedido { get; set; }




    }
}
