using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Ofertas;
using EcomerceCon_Apis.Dtos.Producto;
using EcomerceCon_Apis.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcomerceCon_Apis.Services
{
    public class ProductoService : IProductosService
    {
        private readonly ProductosDbcontext _context;

        public ProductoService(ProductosDbcontext context)
        {
            _context = context;
        }
        public async Task<List<ProductoDtoParaAminds>> ListaDeAdmins()
        {

            var fecha = DateTime.Now;
            try
            {
                var producto = await _context.Productos.Include(c => c.categoria).Include(o => o.ofertas)
                .Select(p => new ProductoDtoParaAminds
                {

                    Id = p.Id,

                    Nombre = p.Nombre,
                    CategoriaId = p.CategoriaId,
                    ImagenUrl = p.ImagenUrl,
                    Cantidad = p.cantidad,
                    tieneoferta = p.ofertas.Any(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin),
                    CategoriaNombre = p.categoria.Nombre,
                    PrecioOriginal = p.precio,
                    PrecioFinal = p.ofertas.Any(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin)
    ? p.precio - (p.precio * (p.ofertas
        .Where(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin)
        .Select(op => op.oferta.MontoAdescontar)
        .FirstOrDefault() / 100)) // <-- Usa 100.0 si tus precios son double/float
    : p.precio

                }).ToListAsync();
                return producto;


            }
            catch (Exception ex)
            {

                throw new Exception("Error al obtener la lista de productos: " + ex.Message);

            }

        }
        public async Task<List<ProductoDto>> lista()
        {
            try
            {


                var aux = await _context.ofertaProductos.Select(p => p.ProductoId).ToListAsync();
                var fechaoferta = await _context.Ofertas.Where(f => f.Fecha_inicio >= DateTime.Now && f.Fecha_fin <= DateTime.Now).FirstOrDefaultAsync();


                if (await _context.Productos.Include(c => c.categoria).ToListAsync() != null)
                {
                    var fecha = DateTime.Now;
                    List<ProductoDto> producto = await _context.Productos.Include(c => c.categoria)
                        .Include(o => o.ofertas).Select(p => new ProductoDto
                        {

                            Id = p.Id,
                            ImagenUrl = p.ImagenUrl,
                            Descripcion = p.Descripcion,
                            Nombre = p.Nombre,
                            CategoriaId = p.CategoriaId,
                            CategoriaNombre = p.categoria.Nombre,
                            PrecioOriginal = p.precio,
                            TieneOferta = p.ofertas.Any(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin),
                            PrecioFinal = p.ofertas.Any(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin)
    ? p.precio - (p.precio * (p.ofertas
        .Where(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin)
        .Select(op => op.oferta.MontoAdescontar)
        .FirstOrDefault() / 100)) 
    : p.precio
                        })
                        .ToListAsync();

                    return producto;
                }

                else
                {
                    throw new Exception("Error al obtener la lista de productos ");

                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la lista de productos: " + ex.Message);
                


            }

        }
        public async Task<List<ListarProductosParaoOfertaDto>> ListarParaOFerta()
        {
            try
            {
                var producto = await _context.Productos.Select(o => new ListarProductosParaoOfertaDto
                {
                    Id = o.Id,
                    Nombre = o.Nombre,
                }
              ).ToListAsync();
                return producto;
            }
            catch (Exception ex)
            {

               throw new Exception("Error al listar productos para oferta: " + ex.Message);
            }
           
     
        }

        public async Task<Producto> Create(CrearProductoDto productoDto)
        {
            var producto = new Producto
            {
                Nombre = productoDto.Nombre,
                Descripcion = productoDto.Descripcion,
                cantidad = productoDto.cantidad,
                precio = productoDto.precio,
                ImagenUrl = productoDto.ImagenUrl,
                CategoriaId = productoDto.CategoriaId
            };
            if (producto.cantidad > 0 && producto.precio > 0)
            {
                return producto;
            }
            else
            {
                throw new Exception("Producto Invalido");

            }




        }

        public async Task<EditarProductoDto> Obtener(int productoId)
        {
            try
            {
                if (productoId >= 1)
                {
                    var producto = await _context.Productos.Include(c => c.categoria).Where(p => p.Id == productoId).FirstOrDefaultAsync();
                    var dto = new EditarProductoDto
                    {
                        Nombre = producto.Nombre,
                        Descripcion = producto.Descripcion,
                        cantidad = producto.cantidad,
                        precio = producto.precio,
                        ImagenUrl = producto.ImagenUrl,
                        CategoriaId = producto.CategoriaId,
                        // Uso del null-conditional por seguridad
                    };

                    
                    return dto;
                }
                else
                {
                    throw new Exception("El Id del producto debe ser mayor a 0.");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el producto: " + ex.Message);
            }

        }
        public async Task<Producto> Editar(EditarProductoDto dto)
        {
            bool prueba = await _context.Productos.AnyAsync(p => p.Id == dto.Id);
            try
            {

                if (prueba==true&&dto.Id>0&&dto.cantidad>0&&dto.precio>0)
                {
                    var produ =
                  new Producto
                  {
                      Id = dto.Id,
                      Nombre = dto.Nombre,
                      Descripcion = dto.Descripcion,
                      cantidad = dto.cantidad,
                      ImagenUrl = dto.ImagenUrl,
                      CategoriaId = dto.CategoriaId,
                      precio = dto.precio,
                  };
                    return produ;
                }
                else
                {
                throw new Exception("Error al obtener el producto");

                }
            }

            catch (Exception ex)
            {

                throw new Exception("Error al obtener el producto: " + ex.Message);

            }



        }



    }
}
