using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos.Ofertas;
using EcomerceCon_Apis.Models;
using Microsoft.EntityFrameworkCore;

namespace EcomerceCon_Apis.Properties.Repository
{
    public class OfertaRepository : IOfertaRepository
    {
        private readonly ProductosDbcontext _context;
        public OfertaRepository(ProductosDbcontext context)
        {
            _context = context;
        }
        public async Task CrearOferta(Ofertas oferta)
        {
            _context.Add(oferta);
            await _context.SaveChangesAsync();
        }

        public async Task CrearRelacionOfertasProducto(IEnumerable<OfertaProducto> ofertaProducto)
        {
            _context.AddRange(ofertaProducto);
            await _context.SaveChangesAsync();
        }

        public async Task<Ofertas> VerificarExistenciaOferta(DateTime fechaInicio, DateTime fechaFin)
        {
          return await _context.Ofertas.FirstOrDefaultAsync(o => fechaInicio <= o.Fecha_fin && fechaFin >= o.Fecha_inicio);
        }

        public async Task<List<Ofertas>> ListarOfertas()
        {
            return await _context.Ofertas.ToListAsync();
        }

        public async Task<Ofertas> ObtenerOfertaPorId(int id)
        {
            return await _context.Ofertas.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task EditarOferta(Ofertas oferta)
        {
            _context.Update(oferta);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarProduDeOferta(List<OfertaProducto> ofertaProducto)
        {
            _context.RemoveRange(ofertaProducto);
            await _context.SaveChangesAsync();
        }

        public async Task<List<int>> ObtenerProductos(int id)
        {
            var algo= await _context.ofertaProductos.Where(op => op.OfertaId == id).Select(p=>p.ProductoId).ToListAsync();
            return algo;
        }

        public async Task<List<OfertaProducto>> Obtenerofertaproducto(int Id)
        {
            return await _context.ofertaProductos.Include(p=>p.producto).Include(o=>o.oferta).Where(op => op.OfertaId == Id).ToListAsync();
        }
    }
}
