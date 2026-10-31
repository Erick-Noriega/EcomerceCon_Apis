using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Pagos;
using EcomerceCon_Apis.Dtos.Producto;
using EcomerceCon_Apis.Models;
using EcomerceCon_Apis.Services;
using MercadoPago.Client.Payment;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EcomerceCon_Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagosController : ControllerBase
    {
    
        private readonly ProductosDbcontext _context;
        private readonly IPagoService _pagoservices;
        public PagosController(ProductosDbcontext context, IPagoService a)
        {
            _context = context;
         
            _pagoservices = a;
        }
        [HttpPost("RegistrarPago")]
        [HttpGet("RegistrarPago")]
        [AllowAnonymous]
        public async Task<IActionResult> RegistrarPago(
     [FromQuery] string? type,
     [FromQuery(Name = "data.id")] string? dataId,
     [FromBody] MercadoPagoWebhookDto? body)

        {
            {
                var pagoId = dataId ?? body?.Data?.Id;

                if (string.IsNullOrEmpty(pagoId))
                    return Ok();
                await _pagoservices.ProcesarPago(
                    type, dataId, body);
                await _pagoservices.VerificarPago(pagoId);
                return Ok();
            }


        }
    }
}


    
