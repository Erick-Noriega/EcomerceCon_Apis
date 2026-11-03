using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Ofertas;
using EcomerceCon_Apis.Dtos.Producto;
using EcomerceCon_Apis.Models;
using EcomerceCon_Apis.Properties.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcomerceCon_Apis.Services
{
    public class ProductoService : IProductosService
    {
        private readonly ProductosDbcontext _context;
        private readonly IProductoRepository _repo;

        public ProductoService(ProductosDbcontext context, IProductoRepository repo)
        {
            _context = context;
            _repo = repo;
        }
        public async Task<List<ProductoDtoParaAminds>> ListaDeAdmins()
        {

            var fecha = DateTime.Now;
          
                var producto = await _repo.ListaDeAdmins();
                var dto = await _context.Productos.Include(c => c.categoria).Include(o => o.ofertas)
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
            if (dto != null)
            {

                return dto;
            }
            else
            {
                throw new Exception("Error al obtener la lista de productos");

            }





        }
        public async Task<List<ProductoDto>> lista()
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

        
        public async Task<List<ListarProductosParaoOfertaDto>> ListarParaOFerta()
        {
            
                var producto = await _context.Productos.Select(o => new ListarProductosParaoOfertaDto
                {
                    Id = o.Id,
                    Nombre = o.Nombre,
                }
              ).ToListAsync();
            if (producto != null)
            {

                return producto;
            }
            else
            {
                throw new Exception("Error al obtener la lista de productos");

            }
        }
            


        

        public async Task Create(CrearProductoDto productoDto)
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
                await _repo.AgregarProducto(producto);
            }
            else
            {
                throw new Exception("Producto Invalido");

            }




        }

        public async Task<EditarProductoDto> Obtener(int productoId)
        {
           
                if (productoId >= 1)
                {
                    var producto = await _repo.Obtener(productoId);



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

                if (dto != null)
                {

                    return dto;
                }
                else
                {
                    throw new Exception("Error al obtener el producto");

                }
            }
                else
                {
                    throw new Exception("El Id del producto debe ser mayor a 0.");
                }

           

        }
        public async Task Editar(EditarProductoDto dto)
        {
            var existencia = await _repo.Obtener(dto.Id);
            
                if (existencia != null && dto.Id > 0 && dto.cantidad > 0 && dto.precio > 0)
                {



                    existencia.Nombre = dto.Nombre;
                    existencia.Descripcion = dto.Descripcion;
                    existencia.cantidad = dto.cantidad;
                    existencia.ImagenUrl = dto.ImagenUrl;
                    existencia.CategoriaId = dto.CategoriaId;
                    existencia.precio = dto.precio;
                    await _repo.GuardarCambios();
                }
                else
                {
                    throw new Exception("Error al obtener el producto");

                }

            }


        }

    }

