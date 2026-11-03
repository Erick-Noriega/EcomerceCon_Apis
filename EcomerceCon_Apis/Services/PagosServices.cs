using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos.Pagos;
using EcomerceCon_Apis.Models;
using EcomerceCon_Apis.Properties.Repository;
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


        private readonly IPedidoRepository _pedido;
        private readonly IWhatsappService whattsapServices;
        private readonly ICarritoService _carritoService;
        private readonly IPagosRepository _pago;
        private readonly PaymentClient _paymentClient;
        public PagosServices(IPedidoRepository context, IMemoryCache i, IWhatsappService w, IPagosRepository pago, ICarritoService carritoService)
        {
            _pedido = context;
            whattsapServices = w;
            _pago = pago;
            _carritoService = carritoService;
            _paymentClient = new PaymentClient();

        }

        public async Task<Preference> GenerarPreferenciaDePago(string pedidoId, Carrito c)
        {
            try
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


                return preference;
            }
            catch (Exception ex)
            {

                throw new Exception("ERROR :" + ex.ToString());
            }



        }


        public async Task ProcesarPago(
     [FromQuery] string? type,
     [FromQuery(Name = "data.id")] string? dataId,
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


                try
                {
                    if (payment != null && payment.Status == "approved")
                    {
                        if (!int.TryParse(payment.ExternalReference, out int pedidoId))
                        {
                            return;
                        }


                        var pedido = await _pedido.ComprobarPedidoConId(pedidoId.ToString());

                        if (pedido == null || pedido.estadoPedido == EstadoPedido.EnPreparacion)
                        {
                            return;
                        }


                        foreach (var detalle in pedido.detallePedido)
                        {
                            if (detalle.producto != null)
                            {
                                detalle.producto.cantidad = Math.Max(0, detalle.producto.cantidad - detalle.Cantidad);

                            }
                        }


                        var nuevoPago = new Pagos
                        {
                            PedidoId = pedido.Id,
                            TransaccionId = payment.Id.ToString(),
                            Monto = (float)(payment.TransactionAmount ?? 0),
                            MetodoPago = payment.PaymentTypeId,
                            FechaPago = DateTime.Now,
                            EstadoPago = payment.Status
                        };
                        if (nuevoPago != null && nuevoPago.Monto > 0)
                        {
                            await _pago.AgregarPago(nuevoPago);

                            await _pago.GuardarCambios();
                        }

                    }
                }
                catch (Exception ex)
                {

                    throw new Exception("ERROR :" + ex.ToString());

                }

                return;
            }
            catch (Exception)
            {
                return;
            }
        }

        public async Task<bool> VerificarPago(string pagoId)
        {
            try
            {
                if (!long.TryParse(pagoId, out long idPago))
                    return false;

                var pago = await _paymentClient.GetAsync(idPago);

                if (pago == null)
                    return false;

                var pedidoId = pago.ExternalReference;

                if (!int.TryParse(pedidoId, out int idPedido))
                    return false;

                var pedido = await _pedido.ComprobarPedidoConId(idPedido.ToString());

                if (pedido == null)
                    return false;

                if (pago.Status == "approved")
                {
                    pedido.estadoPedido = EstadoPedido.EnPreparacion;
                    await _carritoService.VaciarCarritoDelUsuario(pedido.UsuarioId);
                    return true;

                }
                else if (pago.Status == "rejected")
                {
                    return false;
                }
                return false;


            }
            catch (Exception ex)
            {

                throw new Exception("ERROR :" + ex.ToString());

            }
        }
    }
}







    

