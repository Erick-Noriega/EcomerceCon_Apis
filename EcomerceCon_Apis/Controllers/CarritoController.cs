using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Carrito;
using EcomerceCon_Apis.Dtos.Categoria;
using EcomerceCon_Apis.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcomerceCon_Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarritoController : ControllerBase
    {
        private readonly ProductosDbcontext _context;
        public CarritoController(ProductosDbcontext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Agregar")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<CrearCarritoDto>), StatusCodes.Status200OK)]

        public async Task<IActionResult> AgregarCarrito([FromBody]CrearCarritoDto carrito)
        {
            
            var c=new Carrito
            {
                UsuarioId = carrito.UsuarioId,
                Fecha = carrito.Fecha
            };
            if (c != null && _context.Carritos.Where(c=>c.usuario.Id==carrito.UsuarioId)==null)
            {
                _context.Carritos.Add(c);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Carrito agregado correctamente" });
            }
            else
            {
                return BadRequest(new { message = "El carrito ya existe para este usuario" });
            }
            

        }


    }
}
