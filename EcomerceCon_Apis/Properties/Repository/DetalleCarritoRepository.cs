using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos.DetalleCarrito;
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


        public async Task<DetalleCarrito> VerificarExistencia(int idcarrito, int productoId)
        {
            return await _context.DetalleCarritos.FirstOrDefaultAsync(d => d.CarritoId == idcarrito && d.ProductoId == productoId);
        }

        public async Task AgregarDetalleCarrito(DetalleCarrito detalleCarrito)
        {
            _context.Add(detalleCarrito);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarProduDeCarrito(DetalleCarrito detallecarrito)
        {
            _context.DetalleCarritos.Remove(detallecarrito);
            await _context.SaveChangesAsync();
        }
        public async Task GuardarCambios()
        {
            await _context.SaveChangesAsync();
        }
    }
}
