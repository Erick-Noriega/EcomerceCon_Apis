using EcomerceCon_Apis.Properties.Repository;

namespace EcomerceCon_Apis.Services
{
    public class CarritoService: ICarritoService
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly IDetalleCarritoRepository _detalleCarritoRepository;
        public CarritoService(ICarritoRepository carritoRepository, IDetalleCarritoRepository detalleCarritoRepository)
        {
            _carritoRepository = carritoRepository;
            _detalleCarritoRepository = detalleCarritoRepository;
        }
        public async Task VaciarCarritoDelUsuario(string usuarioId)
        {
            var carrito = await _carritoRepository.EncontrarCarrito(usuarioId);
            if (carrito != null)
            {
                await _detalleCarritoRepository.VaciarProductos(carrito);
            }
        }
    }
    
    }

