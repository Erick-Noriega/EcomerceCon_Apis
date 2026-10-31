using EcomerceCon_Apis.Dtos.Pagos;
using Microsoft.AspNetCore.Mvc;

namespace EcomerceCon_Apis.Services
{
    public interface IPagoService
    {

        Task ProcesarPago([FromQuery] string? type,
     [FromQuery(Name = "data.id")] string? dataId,
     [FromBody] MercadoPagoWebhookDto? body);

        Task VaciarCarritoDelUsuario(string usuarioId);
        Task VerificarPago(string pagoId);
    }
}
