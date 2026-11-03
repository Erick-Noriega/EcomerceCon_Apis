using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Models;
using Microsoft.EntityFrameworkCore;

namespace EcomerceCon_Apis.Properties.Repository
{
    public class CarritoRepository : ICarritoRepository
    {
		private readonly ProductosDbcontext _context;
        public CarritoRepository(ProductosDbcontext context)
		{
            _context = context;
        }
        public async Task<Carrito?> EncontrarCarrito(string usuarioId)
        {
			try
			{
				return await _context.Carritos.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
            }
			catch (Exception)
			{

				throw;
            }
        }

        public async Task VaciarCarritoDelUsuario(string usuarioId)
        {
           
                var itemsCarrito = await _context.Carritos.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
                if (itemsCarrito != null)
                {
                    var detallecarrito = await _context.DetalleCarritos.Where(d => d.CarritoId == itemsCarrito.Id).ToListAsync();
                    _context.DetalleCarritos.RemoveRange(detallecarrito);

                }
        
           
        }
    }
}
