using ConsumirapideEcomerceee.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace ConsumirapideEcomerceee.Controllers
{
    public class DetaleCarritoController : Controller
    {
        private readonly HttpClient _httpClient;
        public DetaleCarritoController(IHttpClientFactory htpp)
        {
            _httpClient=htpp.CreateClient("MiApi");
        }
        [HttpGet]
        public async Task<IActionResult> Index()
            {

            var token = User.FindFirstValue("JWToKen");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("api/DetalleCarrito/VerCarrito");
            var json=await response.Content.ReadAsStringAsync();
            var carrito = JsonConvert.DeserializeObject<List<DetalleCarritoDto>>(json);
       
            return View(carrito);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CrearDetalleCarritoDto detalle)
        {
            var token = User.FindFirstValue("JWToKen");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var json=JsonConvert.SerializeObject(detalle);
            var content=new StringContent(json,Encoding.UTF8,"application/json");
            await _httpClient.PostAsync("api/DetalleCarrito/Crear", content);
            return RedirectToAction("Index");
           
        }


    }
}
