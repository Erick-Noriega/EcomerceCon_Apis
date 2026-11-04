using EcomerceCon_Apis.Models;

namespace EcomerceCon_Apis.Properties.Repository
{
    public interface IOfertaRepository
    {
        Task CrearOferta(Ofertas oferta);

        Task CrearRelacionOfertasProducto(IEnumerable<OfertaProducto> ofertaProducto);
        Task<Ofertas> VerificarExistenciaOferta(DateTime fechaInicio, DateTime fechaFin);
        Task<List<Ofertas>> ListarOfertas();
        Task<Ofertas> ObtenerOfertaPorId(int id);
        Task EditarOferta(Ofertas oferta);
        Task EliminarProduDeOferta(List<OfertaProducto> ofertaProducto);
        Task<List<int>> ObtenerProductos(int id);
        Task<List<OfertaProducto>> Obtenerofertaproducto(int idoferta);
    }
}
