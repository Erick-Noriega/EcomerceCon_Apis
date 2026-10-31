using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos.Pagos;
using EcomerceCon_Apis.Models;
using MercadoPago.Client.Payment;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Payment;
using MercadoPago.Resource.Preference;
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

        public async Task<Preference> GenerarPreferenciaDePago(string pedidoId,Carrito c)
        {
            var client = new PreferenceClient();
            var prefencelient = new PreferenceRequest
            {
                ExternalReference = pedidoId,
                Items = c.DetalleCarrito.Select(item => new PreferenceItemRequest
                {
                    Title = item.producto!.Nombre,
                    Quantity = item.cantidad,
                    CurrencyId = "ARS",
                    UnitPrice = (decimal)item.PrecioPorUnidad
                }).ToList(),
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = "https://localhost:5023/Pedido/Pago_exitoso",
                    Failure = "http://localhost:5023/Pedido/Fallido",
                    Pending = "https://localhost:5023/Pedido/Pendiente"
                },


                NotificationUrl = "https://scarcity-growl-sectional.ngrok-free.dev/api/Pagos/RegistrarPago",
                AutoReturn = "approved",
            };
            Preference preference = await client.CreateAsync(prefencelient);
            //await transaction.CommitAsync();

            return preference;


        }


        public async Task ProcesarPago(
     [FromQuery] string? type,
     [FromQuery(Name ="data.id")] string? dataId,
     [FromBody] MercadoPagoWebhookDto? body)
        {
 
             string paymentIdStr = dataId ?? body?.Data?.Id;
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

                    if (ex.Message.Contains("404") || ex.Message.Contains("not found"))
                    {
                        return;
                    }


                    return;
                }

     
                if (payment != null && payment.Status == "approved")
                {
                    if (!int.TryParse(payment.ExternalReference, out int pedidoId))
                    {
                        return ;
                    }

                 
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

                    pedido.estadoPedido = EstadoPedido.EnPreparacion;

                    // Vaciar Carrito
                    await VaciarCarritoDelUsuario(pedido.UsuarioId);

                    await _context.SaveChangesAsync();
      
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

            //   var telefono = await _context.Users
            //       .Where(u => u.Id == pedido.UsuarioId)
            //       .Select(u => u.Numero_telefono)
            //       .FirstOrDefaultAsync();
            string telefono = "543516341130";
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

