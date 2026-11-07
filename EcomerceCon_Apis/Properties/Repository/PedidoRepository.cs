using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Models;
using Microsoft.EntityFrameworkCore;

namespace EcomerceCon_Apis.Properties.Repository
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly ProductosDbcontext _context;
        public PedidoRepository( ProductosDbcontext context)
        {
            _context = context;
        }

        public async Task<Pedido> ComprobarPedidoConId(string idpedido)
        {
            var pedido = await _context.Pedidos
       .Include(p => p.Usuario)
       .Include(p => p.detallePedido)
           .ThenInclude(d => d.producto)
       .FirstOrDefaultAsync(p => p.Id.ToString() == idpedido);
            return pedido;
        }

        public async Task ConfirmarPedido(Pedido pedido)
        {
            _context.Add(pedido);
    
        }

        public async Task GuardarCambios()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Pedido> VerificarExistencia(string UsuarioId, EstadoPedido estadoPedido)
        {
            var pedido=await _context.Pedidos
       .FirstOrDefaultAsync(p => p.UsuarioId == UsuarioId && p.estadoPedido == estadoPedido);
            return pedido;


        }

        public async Task<EstadoPedido> EstadoDePedido(int pedidoId)
        {
            var pedido = await _context.Pedidos.Where(p=>p.Id == pedidoId).Select(p=>p.estadoPedido).FirstOrDefaultAsync();
            return pedido;
        }

        public async Task<Pedido>ObtenerPedidoConId(string idpedido)
        {
          return await _context.Pedidos.FirstOrDefaultAsync(p => p.Id.ToString() == idpedido);
        }
    }
}
