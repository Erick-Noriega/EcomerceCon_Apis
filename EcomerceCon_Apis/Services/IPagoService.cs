using EcomerceCon_Apis.Dtos.Pagos;
using Microsoft.AspNetCore.Mvc;

namespace EcomerceCon_Apis.Services
{
    public interface IPagoService
    {

        Task ProcesarPago([FromQuery] string? type,
     [FromQuery] string? topic,
     [FromQuery] string? id,
     [FromQuery] string? collection_id,
     [FromBody] MercadoPagoWebhookDto? body);

        Task VaciarCarritoDelUsuario(string usuarioId);
        Task VerificarPago(string pagoId);
    }
}
