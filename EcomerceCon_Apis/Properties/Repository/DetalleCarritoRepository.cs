using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Models;
using Microsoft.EntityFrameworkCore;

namespace EcomerceCon_Apis.Properties.Repository
{
    public class DetalleCarritoRepository : IDetalleCarritoRepository
    {
        private readonly ProductosDbcontext _context;
      public DetalleCarritoRepository(ProductosDbcontext context)
        {
            _context = context;
        }
        public async Task VaciarProductos(Carrito c)
        {

            var detallecarrito = await _context.DetalleCarritos.Where(d => d.CarritoId == c.Id).ToListAsync();
            _context.DetalleCarritos.RemoveRange(detallecarrito);
            await _context.SaveChangesAsync();


        }
    }
}
