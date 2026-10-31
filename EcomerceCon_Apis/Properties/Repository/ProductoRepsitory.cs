using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Models;
using Microsoft.EntityFrameworkCore;

namespace EcomerceCon_Apis.Properties.Repository
{
    public class ProductoRepsitory : IProductoRepository
    {

        private readonly ProductosDbcontext _context;
        public ProductoRepsitory(ProductosDbcontext context)
        {
            _context = context;
        }

        public async Task AgregarProducto(Producto producto)
        {
        _context.Add(producto);
            await _context.SaveChangesAsync();
        }

        public async Task Editar(Producto producto)
        {
            _context.Update(producto);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambios()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Producto>> ListaDeAdmins()
            {
            return await _context.Productos
             .Include(p => p.categoria)
             .Include(p => p.ofertas)
                 .ThenInclude(po => po.oferta)
             .ToListAsync();

        }

        public async Task<Producto> Obtener(int Id)
        {
            var producto = await _context.Productos.Include(c => c.categoria).Where(p => p.Id == Id).FirstOrDefaultAsync();
            return producto;
        }
    }
}
