using EcomerceCon_Apis.Models;

namespace EcomerceCon_Apis.Properties.Repository
{
    public interface IDetalleCarritoRepository
    {
        Task VaciarProductos(Carrito id);
        Task AgregarDetalleCarrito(DetalleCarrito detalleCarrito);
        Task<DetalleCarrito> VerificarExistencia(int idcarrito, int productoId);
        Task EliminarProduDeCarrito(DetalleCarrito detallecarrito);
        Task GuardarCambios();


    }
}
