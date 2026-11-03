using EcomerceCon_Apis.Models;

namespace EcomerceCon_Apis.Properties.Repository
{
    public interface IDetalleCarritoRepository
    {
        Task VaciarProductos(Carrito id);
    }
}
