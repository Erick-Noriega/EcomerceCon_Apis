using EcomerceCon_Apis.Models;

namespace EcomerceCon_Apis.Properties.Repository
{
    public interface ICarritoRepository
    {
        Task<Carrito?> EncontrarCarrito(string usuarioId);
       

    }
}
