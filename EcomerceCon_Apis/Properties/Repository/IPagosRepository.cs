using EcomerceCon_Apis.Models;

namespace EcomerceCon_Apis.Properties.Repository
{
    public interface IPagosRepository
    {
        Task AgregarPago(Pagos pago);
        Task GuardarCambios();
        Task<Pagos> ObtenerPagoPorId(int pagoId);
    }
}
