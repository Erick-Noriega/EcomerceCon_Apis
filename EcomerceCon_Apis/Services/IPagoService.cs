using EcomerceCon_Apis.Dtos.Pagos;
using EcomerceCon_Apis.Models;
using MercadoPago.Resource.Preference;
using Microsoft.AspNetCore.Mvc;

namespace EcomerceCon_Apis.Services
{
    public interface IPagoService
    {

        Task ProcesarPago([FromQuery] string? type,
     [FromQuery(Name = "data.id")] string? dataId,
     [FromBody] MercadoPagoWebhookDto? body);

  
     
        Task<Preference> GenerarPreferenciaDePago(string PedidoId,Carrito c);
        Task<bool> VerificarPago(string pagoId);
    }
}
