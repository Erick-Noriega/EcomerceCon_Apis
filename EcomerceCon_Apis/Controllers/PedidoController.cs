using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Pagos;
using EcomerceCon_Apis.Dtos.Pedido;
using EcomerceCon_Apis.Models;
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
    public class PedidoController : ControllerBase
    {
        private readonly ProductosDbcontext _context;
        private readonly UserManager<Usuario> _userManager;
        public PedidoController(ProductosDbcontext context, UserManager<Usuario> U)
        {
            _context = context;
            _userManager = U;
        }
        [HttpPost]
        [Route("ConfirmarPedido")]
        [Authorize]
        [ProducesResponseType(typeof(RespuestaGenericaDto<DireccionDeEnvioDto>), StatusCodes.Status200OK)]

        public async Task<IActionResult>ConfirmarPedido([FromBody]DireccionDeEnvioDto direccion)

        {
            var email = _userManager.GetUserId(User);
            var usuario = await _userManager.FindByEmailAsync(email) ?? await _userManager.FindByEmailAsync(email);
            if (usuario == null)
            {
                return Unauthorized("Usuario no autenticado.");
            }
            var usuarioId = usuario.Id;



            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized("Usuario no autenticado.");
            }

            var carrito = await _context.Carritos.Include(c => c.DetalleCarrito).ThenInclude(d => d.producto).FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
            if (carrito == null||carrito.DetalleCarrito==null||!carrito.DetalleCarrito.Any())
            {
                return NotFound("Carrito no encontrado");
            }
            using var transaction = await _context.Database.BeginTransactionAsync();
            var pedido = new Pedido
            {

                UsuarioId = usuarioId,
                fecha_pedido = DateTime.Now,
                total = (float)carrito.DetalleCarrito.Sum(d => d.PrecioPorUnidad * d.cantidad),
                estadoPedido = EstadoPedido.Pendiente,
                DireccionDeEnvio= direccion.DireccionCompleta,
                detallePedido = carrito.DetalleCarrito.Select(d => new DetallePedido
                {
                    ProductoId = d.ProductoId,
                    Cantidad = d.cantidad,
                    PrecioPorUni = d.PrecioPorUnidad,
                   
                   
                }).ToList()

            };
            var estadoepdido = EstadoPedido.Pendiente;
            var comprobar = await _context.Pedidos
      .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && p.estadoPedido == estadoepdido);
            if (comprobar == null)
            {

            _context.Pedidos.Add(pedido);
            }
            else if(comprobar!=null)
            {
                pedido.Id=comprobar.Id;
            }
                await _context.SaveChangesAsync();
          

            var client = new PreferenceClient();
            var prefencelient = new PreferenceRequest
            {
                ExternalReference = pedido.Id.ToString(),
                Items = carrito.DetalleCarrito.Select(item => new PreferenceItemRequest
                {
                    Title = item.producto!.Nombre,
                    Quantity = item.cantidad,
                    CurrencyId = "ARS",
                    UnitPrice = (decimal)item.PrecioPorUnidad
                }).ToList(),
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = "https://localhost:5023/Pedido/Pago_exitoso",
                    Failure = "https://localhost:5023/Pedido/Fallido",
                    Pending= "https://localhost:5023/Pedido/Pendiente"
                },
                

                NotificationUrl = "https://scarcity-growl-sectional.ngrok-free.dev/api/Pagos/RegistrarPago",
                AutoReturn = "approved",
            };
            Preference preference = await client.CreateAsync(prefencelient);
            await transaction.CommitAsync();

            return Ok(new
            {
                pedidoId = pedido.Id,
                initPoint = preference.InitPoint
            });


            
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
