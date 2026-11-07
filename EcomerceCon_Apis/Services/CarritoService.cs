using EcomerceCon_Apis.Dtos.DetalleCarrito;
using EcomerceCon_Apis.Dtos.Producto;
using EcomerceCon_Apis.Models;
using EcomerceCon_Apis.Properties.Repository;
using MercadoPago.Resource.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcomerceCon_Apis.Services
{
    public class CarritoService : ICarritoService
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly IDetalleCarritoRepository _detalleCarritoRepository;
        private readonly IProductosService _producto;
        public CarritoService(ICarritoRepository carritoRepository, IDetalleCarritoRepository detalleCarritoRepository, IProductosService producto)
        {
            _carritoRepository = carritoRepository;
            _detalleCarritoRepository = detalleCarritoRepository;
            _producto = producto;
        }
        public async Task VaciarCarritoDelUsuario(string usuarioId)
        {
            var carrito = await _carritoRepository.EncontrarCarrito(usuarioId);
            if (carrito != null)
            {
                await _detalleCarritoRepository.VaciarProductos(carrito);
            }
        }

        public async Task AgregarDetalleCarrito(CrearDetalleCarritoDto crearDetalleCarritoDto, string usuarioId)
        {
            try
            {
                var fecha = DateTime.Now;
                // var productoDb = await _producto.Obtener(crearDetalleCarritoDto.ProductoId);


              var producto = await _producto.Obtener(crearDetalleCarritoDto.ProductoId);

                var precioFinal = producto.precio;


                var carrito = await _carritoRepository.EncontrarCarrito(usuarioId);


                if (carrito == null)
                {
                    throw new Exception("El Usuario No tiene Carrito");
                }
                var existencia = await _detalleCarritoRepository.VerificarExistencia(carrito.Id, crearDetalleCarritoDto.ProductoId);
                if (existencia != null)
                {
                    existencia.cantidad += crearDetalleCarritoDto.cantidad;
                    if (existencia.cantidad <= 0)
                    {
                        await _detalleCarritoRepository.EliminarProduDeCarrito(existencia);

                    }
                    else
                    {
                        await _detalleCarritoRepository.GuardarCambios();


                    }
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
                    await _detalleCarritoRepository.AgregarDetalleCarrito(NuevoDetalle);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar detalle al carrito: " + ex.Message);
            }
        }
    }
}

    
    

