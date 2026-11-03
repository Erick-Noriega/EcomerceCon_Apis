using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Models;
using Microsoft.EntityFrameworkCore;

namespace EcomerceCon_Apis.Properties.Repository
{
    public class PagosRepository : IPagosRepository
    {
        private readonly ProductosDbcontext _context;
        public PagosRepository(ProductosDbcontext context)
        {
            _context = context;
        }
        public async Task AgregarPago(Pagos pago)
        {
               _context.Add(pago);
         
        }

        public async Task GuardarCambios()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Pagos> ObtenerPagoPorId(int pagoId)
        {
            var pago= await _context.Pagos.FirstOrDefaultAsync(p => p.Id == pagoId);
            return pago;

           
        }
    }
}
