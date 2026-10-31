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
    }
}
