using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos.Pagos;
using EcomerceCon_Apis.Models;
using MercadoPago.Client.Payment;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EcomerceCon_Apis.Services
{
    public class PagosServices : IPagoService
    {

        private readonly IMemoryCache _memoryCache;
        private readonly ProductosDbcontext _context;
        private readonly IWhatsappService whattsapServices;
        private readonly PaymentClient _paymentClient;
        public PagosServices(ProductosDbcontext context, IMemoryCache i, IWhatsappService w)
        {
            _context = context;
            _memoryCache = i;
            whattsapServices= w;
            _paymentClient = new PaymentClient();
               
        }
        public async Task ProcesarPago(
     [FromQuery] string? type,
     [FromQuery] string? topic,
     [FromQuery] string? id,
     [FromQuery] string? collection_id,
     [FromBody] MercadoPagoWebhookDto? body)
        {
            string paymentIdStr = id ?? collection_id ?? body?.Data?.Id;

            if (string.IsNullOrEmpty(paymentIdStr))
            {
                return;
            }

            try
            {
                if (!long.TryParse(paymentIdStr, out long paymentId))
                {
                    return;
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
                        return ;
                    }

                    // Si es otro tipo de error, lo volvemos a lanzar o respondemos OK según tu estrategia
                    return;
                }

                // Si la API devolvió un pago válido y está aprobado
                if (payment != null && payment.Status == "approved")
                {
                    if (!int.TryParse(payment.ExternalReference, out int pedidoId))
                    {
                        return ;
                    }

                    using var transaction = await _context.Database.BeginTransactionAsync();
                    var pedido = await _context.Pedidos
     .Include(p => p.detallePedido)
         .ThenInclude(d => d.producto)
     .FirstOrDefaultAsync(p => p.Id == pedidoId);

                    if (pedido == null || pedido.estadoPedido == EstadoPedido.EnPreparacion)
                    {
                        return ;
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

                return;
            }
            catch (Exception)
            {
                return ;
            }
        }
        public async Task VaciarCarritoDelUsuario(string usuarioId)
        {
            // Buscas todos los elementos del carrito de ese usuario
            var itemsCarrito = await _context.Carritos.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
            if (itemsCarrito != null)
            {
                var detallecarrito = await _context.DetalleCarritos.Where(d => d.CarritoId == itemsCarrito.Id).ToListAsync();
                _context.DetalleCarritos.RemoveRange(detallecarrito);




            }
        }



        public async Task VerificarPago(string pagoId)
        {
            if (!long.TryParse(pagoId, out long idPago))
                return;

            var pago = await _paymentClient.GetAsync(idPago);

            if (pago == null)
                return;

            var pedidoId = pago.ExternalReference;

            if (!int.TryParse(pedidoId, out int idPedido))
                return;

            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(p => p.Id == idPedido);

            if (pedido == null)
                return;

            var telefono = await _context.Users
                .Where(u => u.Id == pedido.UsuarioId)
                .Select(u => u.Numero_telefono)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(telefono))
                return;

            if (pago.Status == "approved")
            {
                await whattsapServices.EnviarRespuesta(
                    telefono,
                    "¡Tu pago fue aprobado! Gracias por tu compra."
                );
            }
            else if (pago.Status == "rejected")
            {
                await whattsapServices.EnviarRespuesta(
                    telefono,
                    "Tu pago fue rechazado. Podés intentar nuevamente."
                );
            }
        }
    }







    }

