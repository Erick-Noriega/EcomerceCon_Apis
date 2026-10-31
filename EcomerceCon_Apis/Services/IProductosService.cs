using EcomerceCon_Apis.Dtos.Ofertas;
using EcomerceCon_Apis.Dtos.Producto;
using EcomerceCon_Apis.Models;

namespace EcomerceCon_Apis.Services
{
    public interface IProductosService
    {
         Task<List<ProductoDtoParaAminds>> ListaDeAdmins();
        Task<List<ProductoDto>> lista();
        Task <List<ListarProductosParaoOfertaDto>>ListarParaOFerta();
        Task<Producto> Create(CrearProductoDto productoDto);
        Task<EditarProductoDto> Obtener(int Id);
        Task <Producto>Editar(EditarProductoDto dto);

    }
}
