using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Pagos;
using EcomerceCon_Apis.Dtos.Producto;
using EcomerceCon_Apis.Models;
using MercadoPago.Client.Payment;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcomerceCon_Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
        private readonly ProductosDbcontext _context;
        public PagosController(ProductosDbcontext context)
        {
            _context = context;
        }
        [HttpPost("RegistrarPago")]
        [HttpGet("RegistrarPago")]
        [AllowAnonymous]
        public async Task<IActionResult> RegistrarPago(
     [FromQuery] string? type,
     [FromQuery] string? topic,
     [FromQuery] string? id,
     [FromQuery] string? collection_id,
     [FromBody] MercadoPagoWebhookDto? body)
        {
            string paymentIdStr = id ?? collection_id ?? body?.Data?.Id;

            if (string.IsNullOrEmpty(paymentIdStr))
            {
                return Ok();
            }

            try
            {
                if (!long.TryParse(paymentIdStr, out long paymentId))
                {
                    return Ok();
                }

                var paymentClient = new PaymentClient();
                Payment payment = null;

                // Intentar consultar el pago
                try
                {
                    payment = await paymentClient.GetAsync(paymentId);
                }
                catch (Exception ex)
                {
                    // Si da error 404 o "Payment not found", es una notificación secundaria (ej. merchant_order)
                    // Respondemos 200 OK para que MP no reintente este ID y esperamos la notificación del pago real
                    if (ex.Message.Contains("404") || ex.Message.Contains("not found"))
                    {
                        return Ok();
                    }

                    // Si es otro tipo de error, lo volvemos a lanzar o respondemos OK según tu estrategia
                    return Ok();
                }

                // Si la API devolvió un pago válido y está aprobado
                if (payment != null && payment.Status == "approved")
                {
                    if (!int.TryParse(payment.ExternalReference, out int pedidoId))
                    {
                        return Ok();
                    }

                    using var transaction = await _context.Database.BeginTransactionAsync();
                    var pedido = await _context.Pedidos
     .Include(p => p.detallePedido)
         .ThenInclude(d => d.producto)
     .FirstOrDefaultAsync(p => p.Id == pedidoId);

                    if (pedido == null || pedido.estadoPedido == EstadoPedido.EnPreparacion)
                    {
                        return Ok();
                    }

                    // Descontar Stock
                    foreach (var detalle in pedido.detallePedido)
                    {
                        if (detalle.producto != null)
                        {
                            detalle.producto.cantidad = Math.Max(0, detalle.producto.cantidad - detalle.Cantidad);
                        }
                    }

                    // Registrar en la tabla Pagos
                    var nuevoPago = new Pagos
                    {
                        PedidoId = pedido.Id,
                        TransaccionId = payment.Id.ToString(),
                        Monto = (float)(payment.TransactionAmount ?? 0),
                        MetodoPago = payment.PaymentTypeId,
                        FechaPago = DateTime.Now,
                        EstadoPago = payment.Status
                    };

                    _context.Pagos.Add(nuevoPago);

                    // Actualizar Estado del Pedido
                    pedido.estadoPedido = EstadoPedido.EnPreparacion;

                    // Vaciar Carrito
                    await VaciarCarritoDelUsuario(pedido.UsuarioId);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }

                return Ok();
            }
            catch (Exception)
            {
                return Ok();
            }
        }

        private async Task VaciarCarritoDelUsuario(string usuarioId)
        {
            // Buscas todos los elementos del carrito de ese usuario
            var itemsCarrito = await _context.Carritos.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
                if (itemsCarrito != null)
            {
                var detallecarrito = await _context.DetalleCarritos.Where(d => d.CarritoId== itemsCarrito.Id).ToListAsync();
                _context.DetalleCarritos.RemoveRange(detallecarrito);

         


            }
        }
    }
}


    
