using EcomerceCon_Apis.Dtos.Ofertas;

namespace EcomerceCon_Apis.Services
{
    public interface IOfertaService
    {

        Task CrearOferta(CrearOfertaDto ofertaDto);
        Task<List<OfertaDto>> ListarOfertas();
        Task<OfertaDto> ObtenerOfertaPorId(int id);
        Task EditarOferta(OfertaDto oferta);
        Task Agregar(AgregarProductosAofertaDto agregarProductosAofertaDto);
        Task EliminarProductosDeLaOferta(AgregarProductosAofertaDto productos);
        Task<List<ListarOfertaProductoDto>> obtenerproductos(int idoferta);
    }
}
