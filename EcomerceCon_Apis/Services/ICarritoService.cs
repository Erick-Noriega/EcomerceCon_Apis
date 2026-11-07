using EcomerceCon_Apis.Dtos.DetalleCarrito;

namespace EcomerceCon_Apis.Services
{
    public interface ICarritoService
    {
        Task VaciarCarritoDelUsuario(string usuarioId);
        Task AgregarDetalleCarrito(CrearDetalleCarritoDto crearDetalleCarritoDto, string usuarioId);
    }
}
