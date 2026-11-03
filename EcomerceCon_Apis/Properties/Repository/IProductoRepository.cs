using EcomerceCon_Apis.Dtos.Producto;
using EcomerceCon_Apis.Models;

namespace EcomerceCon_Apis.Properties.Repository
{
    public interface IProductoRepository
    {


        Task<List<Producto>> ListaDeAdmins();
        Task AgregarProducto(Producto producto);
        Task<Producto> Obtener(int Id);
        Task Editar(Producto producto);
    
        Task GuardarCambios();

    }
}
