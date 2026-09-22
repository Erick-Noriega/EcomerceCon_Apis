namespace EcomerceCon_Apis.Models
{
    public class DetallePedido
    {

        public int Id { get; set; }
        public Pedido? Pedido { get; set; }
        public int PedidoId { get; set; }
        public int ProductoId { get; set; }
        public Producto? producto { get; set; }
        public DateTime Fecha { get; set; }
        public float PrecioPorUni { get; set; }
        public int Cantidad { get; set; }

    }
}
