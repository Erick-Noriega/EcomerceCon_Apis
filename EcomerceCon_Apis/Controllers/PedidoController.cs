using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Pagos;
using EcomerceCon_Apis.Dtos.Pedido;
using EcomerceCon_Apis.Models;
using EcomerceCon_Apis.Services;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace EcomerceCon_Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
        [Authorize(Roles = "Admin")]
    public class PedidoController : ControllerBase
    {
        private readonly ProductosDbcontext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly IPedidoService _pedidos;
   
        public PedidoController(ProductosDbcontext context, UserManager<Usuario> U, IPedidoService pedido)
        {
            _context = context;
            _userManager = U;
            _pedidos = pedido;
          
        }
        [HttpPost]
        [Route("ConfirmarPedido")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<DireccionDeEnvioDto>), StatusCodes.Status200OK)]

        public async Task<IActionResult>ConfirmarPedido([FromBody]DireccionDeEnvioDto direccion)

        {
            var usuarioId = _userManager.GetUserId(User);

            var res = await _pedidos.AgregarPedido(direccion, usuarioId);


            return Ok(res);


            
        }
        [HttpPut]
        [Route("ActualizarEstado")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromBody] string nuevoEstado)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
           return NotFound("Pedido no encontrado");
            }
            else if (pedido != null)
            {
                if (nuevoEstado.ToLower() is not ("pendiente" or "enviado" or "entregado"))
                {
                    return BadRequest("El estado ingresado no es válido.");
                }

                pedido.estadoPedido = nuevoEstado.ToLower() switch
                {
                    "pendiente" => EstadoPedido.Pendiente,
                    "enviado" => EstadoPedido.Enviado,
                    "entregado" => EstadoPedido.Entregado,
                    _ => pedido.estadoPedido
                };
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Estado del pedido actualizado exitosamente", estadoActualizado = pedido.estadoPedido.ToString() });

            }
            return BadRequest("No se pudo actualizar el estado del pedido.");


        }

    }
}
