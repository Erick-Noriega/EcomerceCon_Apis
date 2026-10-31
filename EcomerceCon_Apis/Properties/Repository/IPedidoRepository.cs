using EcomerceCon_Apis.Models;

namespace EcomerceCon_Apis.Properties.Repository
{
    public interface IPedidoRepository
    {
        Task ConfirmarPedido(Pedido pedido);
        Task GuardarCambios();
        Task<Pedido> VerificarExistencia(string UsuarioId, EstadoPedido estadoPedido);


    }
}
