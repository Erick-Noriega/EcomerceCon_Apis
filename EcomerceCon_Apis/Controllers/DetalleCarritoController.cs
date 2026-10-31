
using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.DetalleCarrito;
using EcomerceCon_Apis.Dtos.Ofertas;
using EcomerceCon_Apis.Dtos.Producto;
using EcomerceCon_Apis.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcomerceCon_Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleCarritoController : ControllerBase
    {
        private readonly ProductosDbcontext _context;
        private readonly UserManager<Usuario>_userManager;
        public DetalleCarritoController(ProductosDbcontext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        [HttpPost]
        [Route("Crear")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<CrearDetalleCarritoDto>), StatusCodes.Status200OK)]

        public async Task<IActionResult> CrearDetalleCarrito([FromBody]CrearDetalleCarritoDto crearDetalleCarritoDto)
        {
            try
            {
                var fecha=DateTime.Now;
                var  productoDb=await _context.Productos.Include(o=>o.ofertas).Where(p=>p.Id==crearDetalleCarritoDto.ProductoId)
                    .FirstOrDefaultAsync();

                List<ProductoDto> producto = await _context.Productos.Include(c => c.categoria)
                     .Include(o => o.ofertas).Where(p=>p.Id==crearDetalleCarritoDto.ProductoId).Select(p => new ProductoDto
                     {
      
                         PrecioFinal = p.ofertas.Any(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin)
 ? p.precio - (p.precio * (p.ofertas
     .Where(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin)
     .Select(op => op.oferta.MontoAdescontar)
     .FirstOrDefault() / 100)) // <-- Usa 100.0 si tus precios son double/float
 : p.precio
                     })
                     .ToListAsync();

                var precioFinal=producto.Select(p=>p.PrecioFinal).FirstOrDefault();

                var usuarioId = _userManager.GetUserId(User);
                var carrito=await _context.Carritos.FirstOrDefaultAsync(c=>c.UsuarioId==usuarioId);
                if (carrito == null)
                {
                    return BadRequest("El Usuario No tiene Carrito");
                }
                var detalleExistente=await _context.DetalleCarritos.FirstOrDefaultAsync(d=>d.CarritoId==carrito.Id&&d.ProductoId== crearDetalleCarritoDto.ProductoId);

                if (detalleExistente != null)
                {
                    detalleExistente.cantidad += crearDetalleCarritoDto.cantidad;
                    if (detalleExistente.cantidad <= 0)
                    {
                        _context.DetalleCarritos.Remove(detalleExistente);
                        await _context.SaveChangesAsync();
                        return Ok();
                    }
                    else
                    {

                      _context.DetalleCarritos.Update(detalleExistente);
                    }
                    detalleExistente.PrecioPorUnidad=precioFinal;
                }
                else
                {
                    var NuevoDetalle = new DetalleCarrito
                    {
                        CarritoId = carrito.Id,
                        ProductoId = crearDetalleCarritoDto.ProductoId,
                        cantidad = crearDetalleCarritoDto.cantidad,
                        PrecioPorUnidad = precioFinal
                    };
                     _context.DetalleCarritos.Add(NuevoDetalle);

                }
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Producto agregado correctamente al carrito." });
            }
            catch (Exception ex )
            {

                return BadRequest(new { message = "error " + ex.Message.ToString() });
            }
            

        }
        [HttpGet]
        [Route("VerCarrito")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<DetalleCarritoDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Lista()
        {

            try
            {
                var idusuario = _userManager.GetUserId(User);


                var ca = await _context.Carritos.FirstOrDefaultAsync(c => c.UsuarioId == idusuario);


                var carrito = await _context.DetalleCarritos.Where(c => c.CarritoId == ca.Id).Select(c => new DetalleCarritoDto
                {
                    Id = c.Id,
                    ProductoId = c.ProductoId,
                    NombreProducto = c.producto.Nombre,
                    ImagenUrl = c.producto.ImagenUrl,
                    PrecioUnitario = c.PrecioPorUnidad,
                    Cantidad = c.cantidad,

                })
                    .ToListAsync();

                return Ok(carrito);
            }
            catch (Exception)
            {

                return Unauthorized();
            }
            
          
        }

    }
}
