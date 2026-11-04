using EcomerceCon_Apis.Models;

namespace EcomerceCon_Apis.Properties.Repository
{
    public interface ICarritoRepository
    {
        Task<Carrito?> EncontrarCarrito(string usuarioId);
        Task CrearCarrito(Carrito carrito);
        Task VaciarCarritoDelUsuario(string usuarioId);



    }
}
