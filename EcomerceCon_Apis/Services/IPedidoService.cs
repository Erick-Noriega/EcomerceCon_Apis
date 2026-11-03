using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Pedido;
using EcomerceCon_Apis.Models;

namespace EcomerceCon_Apis.Services
{
    public interface IPedidoService
    {
        Task<PedidoPagoDTo> AgregarPedido(DireccionDeEnvioDto dto,string UserId);
        Task<Pedido> ObtenerPedidoConId(string idpedido);
 
        



    }
}
