using ConsumirapideEcomerceee.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace ConsumirapideEcomerceee.Controllers
{
    public class PedidoController : Controller
    {
        private readonly HttpClient _htppclient;
        [HttpGet]

        public IActionResult Pago_Exitoso()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Fallido()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Pendiente()
        {
            return View();
        }

        public PedidoController(IHttpClientFactory htppclient)
        {
            _htppclient = htppclient.CreateClient("MiApi");
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmarPedido([FromBody] DireccionDeEnviosDto dto)
        {
            var token = User.FindFirstValue("JWToKen");
            _htppclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _htppclient.PostAsync("api/Pedido/ConfirmarPedido", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();

                // 1. Deserializamos el JSON devuelto por la API
                var resultado = JsonConvert.DeserializeObject<RespuestaPedidoDto>(responseData);

                // 2. Redirigimos la ventana del usuario a Mercado Pago
                if (resultado != null && !string.IsNullOrEmpty(resultado.InitPoint))
                {
                    return Ok(new { initPoint = resultado.InitPoint });
                }
            }

            return BadRequest("No se pudo procesar el pago.");
        }

        // DTO interno para mapear la respuesta de la API
        public class RespuestaPedidoDto
        {
            public int PedidoId { get; set; }
            public string InitPoint { get; set; } = string.Empty;
        }

    }
}
