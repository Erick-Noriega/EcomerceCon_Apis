using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Pedido;
using EcomerceCon_Apis.Models;
using EcomerceCon_Apis.Properties.Repository;
using MercadoPago.Resource.Preference;
using MercadoPago.Resource.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace EcomerceCon_Apis.Services
{
    public class PedidoServices : IPedidoService
    {
        private readonly ProductosDbcontext _context;
        private readonly IPedidoRepository _repo;
        private readonly IPagoService _pago;
        public PedidoServices(ProductosDbcontext user, IPedidoRepository p,IPagoService pago)
        {
            _context = user;
            _repo = p;
            _pago = pago;
        }
        public async Task<PedidoPagoDTo> AgregarPedido(DireccionDeEnvioDto dto,string UserId)
        {
            if (string.IsNullOrEmpty(UserId))
            {
               
            }
            var carrito = await _context.Carritos.Include(c => c.DetalleCarrito).ThenInclude(d => d.producto).FirstOrDefaultAsync(c => c.UsuarioId == UserId);
            if (carrito == null || carrito.DetalleCarrito == null || !carrito.DetalleCarrito.Any())
            {
                return new PedidoPagoDTo
                {
                    Exito = false,
                    mensaje="no existe el carrito",
                };
            }
            using var transaction = await _context.Database.BeginTransactionAsync();
            var pedido = new Pedido
            {

                UsuarioId = UserId,
                fecha_pedido = DateTime.Now,
                total = (float)carrito.DetalleCarrito.Sum(d => d.PrecioPorUnidad * d.cantidad),
                estadoPedido = EstadoPedido.Pendiente,
                DireccionDeEnvio = dto.DireccionCompleta,
                detallePedido = carrito.DetalleCarrito.Select(d => new DetallePedido
                {
                    ProductoId = d.ProductoId,
                    Cantidad = d.cantidad,
                    PrecioPorUni = d.PrecioPorUnidad,


                }).ToList()
            };
            var existencia = await _repo.VerificarExistencia(UserId, pedido.estadoPedido);
            var preference = new Preference();
            if (existencia == null)
            {
               await _repo.ConfirmarPedido(pedido);
                await _repo.GuardarCambios();
            preference = await _pago.GenerarPreferenciaDePago(pedido.Id.ToString(), carrito);
            }
            else 
            {
                
                existencia.DireccionDeEnvio = pedido.DireccionDeEnvio;
                existencia.detallePedido = pedido.detallePedido;
                existencia.fecha_pedido = pedido.fecha_pedido;
                existencia.total = pedido.total;
                await _repo.GuardarCambios();
           preference = await _pago.GenerarPreferenciaDePago(existencia.Id.ToString(), carrito);

            }
           
            await transaction.CommitAsync();
            var respuesta = new PedidoPagoDTo
            {
                PedidoId = pedido.Id,
                InitPoint = preference.InitPoint,
                mensaje="Link generado",
                Exito=true
            };         
            return respuesta;

        }
        public async Task<Pedido> ObtenerPedidoConId(string idpedido)
        {
           var pedido = await _repo.ObtenerPedidoConId(idpedido);
            if(pedido == null)
            {
                throw new ArgumentException("El pedido no existe");
            }
            return pedido;
        }

        public async Task<EstadoPedido>EstadoDePedido(int pedidoId)
        {
            var estado = await _repo.EstadoDePedido(pedidoId);
            return estado;
        }
    }

    }




    