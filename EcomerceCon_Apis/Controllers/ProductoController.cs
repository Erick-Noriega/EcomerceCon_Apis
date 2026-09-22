using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Ofertas;
using EcomerceCon_Apis.Dtos.Producto;
using EcomerceCon_Apis.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EcomerceCon_Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ProductosDbcontext _context;

        public ProductoController(ProductosDbcontext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("ListaParaAdmins")]
   
        [ProducesResponseType(typeof(RespuestaGenericaDto<List<ProductoDtoParaAminds>>), StatusCodes.Status200OK)]

        public async Task<IActionResult> ListaDeAdmins()
        {

            var fecha = DateTime.Now;
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
                    PrecioFinal= p.ofertas.Any(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin)
    ? p.precio - (p.precio * (p.ofertas
        .Where(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin)
        .Select(op => op.oferta.MontoAdescontar)
        .FirstOrDefault() / 100)) // <-- Usa 100.0 si tus precios son double/float
    : p.precio

                }).ToListAsync();
            return Ok(producto);



        }


        [HttpGet]
        [Route("Lista")]
       
        [ProducesResponseType(typeof(RespuestaGenericaDto<List<ProductoDto>>), StatusCodes.Status200OK)]

        public async Task<IActionResult> listar()
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
                            ImagenUrl=p.ImagenUrl,
                            Descripcion=p.Descripcion,
                            Nombre = p.Nombre,
                            CategoriaId = p.CategoriaId,
                            CategoriaNombre = p.categoria.Nombre,
                            PrecioOriginal = p.precio,
                            TieneOferta = p.ofertas.Any(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin),
                            PrecioFinal = p.ofertas.Any(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin)
    ? p.precio - (p.precio * (p.ofertas
        .Where(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin)
        .Select(op => op.oferta.MontoAdescontar)
        .FirstOrDefault() / 100)) // <-- Usa 100.0 si tus precios son double/float
    : p.precio
                        })
                        .ToListAsync();
                   
                    return Ok(producto);
                }
                
                else
                {
                    return NotFound(new { message = "No se encontraron productos" });
                }

            }
            catch (Exception ex)
            {

                return Ok(new { message = $"error{ex.Message}" });


            }

        }
        [HttpGet]
        [Route("ListarParaOferta")]
        public async Task<IActionResult> ListaNorma()
        {
          var producto=await _context.Productos.Select(o=>new ListarProductosParaoOfertaDto
            {
               Id=o.Id,
               Nombre=o.Nombre,
            }
                ).ToListAsync();
            return Ok(producto);

        }

        [HttpPost]
        [Route("Crear")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<CrearProductoDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Crear([FromBody] CrearProductoDto productoDto)
        {
            try
            {
                if (await _context.Productos.Where(n => n.Nombre == productoDto.Nombre).FirstOrDefaultAsync() == null)
                {

                    if(productoDto.cantidad>0&&productoDto.precio>0)
                    _context.Productos.Add(new Models.Producto
                    {
                        Nombre = productoDto.Nombre,
                        Descripcion = productoDto.Descripcion,
                        cantidad = productoDto.cantidad,
                        ImagenUrl = productoDto.ImagenUrl,
                        precio = productoDto.precio,
                        CategoriaId = productoDto.CategoriaId

                    });
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Producto creado correctamente" });
                }
                else if (await _context.Productos.Where(n => n.Nombre == productoDto.Nombre).FirstOrDefaultAsync() != null)
                {
                    var pelota = await _context.Productos.Where(n => n.Nombre == productoDto.Nombre).FirstOrDefaultAsync();
                    return Ok(new { message = "El producto ya existe" });
                }
                else
                {
                    return Ok(new { message = "Intente mas tarde" });

                }

            }
            catch (Exception ex)
            {

                {
                    return Ok(new { message = $"error{ex.Message}" });

                }
            }

        }
        [HttpGet]
        [Route("Obtener/{Id:int}")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<EditarProductoDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Obtener(int Id)
        {
            try
            {
                var producto = await _context.Productos.Include(c => c.categoria).Where(p => p.Id == Id).FirstOrDefaultAsync();
                var dto = new RespuestaGenericaDto<EditarProductoDto>
                {
                    IsSuccess = true,
                    Mensaje = "Producto encontrado",
                    Data = new EditarProductoDto
                    {
                        Nombre = producto.Nombre,
                        Descripcion = producto.Descripcion,
                        cantidad = producto.cantidad,
                        precio = producto.precio,
                        ImagenUrl = producto.ImagenUrl,
                        CategoriaId = producto.CategoriaId,
                        // Uso del null-conditional por seguridad
                    }

                };
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return Ok(new RespuestaGenericaDto<EditarProductoDto>
                {
                    IsSuccess = false,
                    Mensaje = "producto no encontrado"
                });
            }







        }
        [HttpPut]
        [Route("Editar")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<EditarProductoDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Editar([FromBody] EditarProductoDto dto)
        {
            bool prueba = await _context.Productos.AnyAsync(p => p.Id == dto.Id);

            if (prueba)
            {


                _context.Update
                        (
                      new Producto
                      {
                          Id = dto.Id,
                          Nombre = dto.Nombre,
                          Descripcion = dto.Descripcion,
                          cantidad = dto.cantidad,
                          ImagenUrl = dto.ImagenUrl,
                          CategoriaId = dto.CategoriaId,
                          precio = dto.precio
                      }
                      );
                var verificar = await _context.Productos.Where(n => n.Nombre == dto.Nombre).FirstOrDefaultAsync();
                if (verificar == null && dto.cantidad > 0 && dto.precio > 0)
                {
                await _context.SaveChangesAsync();
                    return Ok(new RespuestaGenericaDto<bool>
                    {
                        IsSuccess = true,
                        Mensaje = "Producto Editado Correctamente",
                        Data = true,
                    });
                }
                return Ok(new RespuestaGenericaDto<bool>
                {
                    IsSuccess = false,
                    Mensaje = "Hubo un error al editar el producto",
               
                });
            }
            return Ok();
        }












    }
}
