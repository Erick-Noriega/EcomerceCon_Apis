
using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.DetalleCarrito;
using EcomerceCon_Apis.Dtos.Ofertas;
using EcomerceCon_Apis.Dtos.Producto;
using EcomerceCon_Apis.Models;
using EcomerceCon_Apis.Properties.Repository;
using EcomerceCon_Apis.Services;
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
        private readonly ICarritoService _carritoService;
        private readonly UserManager<Usuario>_userManager;
        public DetalleCarritoController(ProductosDbcontext context, UserManager<Usuario> userManager, ICarritoService P)
        {
            _context = context;
            _userManager = userManager;
            _carritoService = P;
        }
        [Authorize]
        [HttpPost]
        [Route("Crear")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<CrearDetalleCarritoDto>), StatusCodes.Status200OK)]

        public async Task<IActionResult> CrearDetalleCarrito([FromBody]CrearDetalleCarritoDto crearDetalleCarritoDto)
        {
            
                var usuarioId = _userManager.GetUserId(User);
            await _carritoService.AgregarDetalleCarrito(crearDetalleCarritoDto, usuarioId);
            return Ok(new { message = "Producto agregado al carrito correctamente" });



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
