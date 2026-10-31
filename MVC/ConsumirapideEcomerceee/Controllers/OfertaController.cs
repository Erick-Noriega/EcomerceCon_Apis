using ConsumirapideEcomerceee.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace ConsumirapideEcomerceee.Controllers
{
    public class OfertaController : Controller
    {
        private readonly HttpClient _httpClient;
        public OfertaController(IHttpClientFactory htpp)
        {
            _httpClient = htpp.CreateClient("MiApi");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/api/Ofertas/Lista");
            var content=await response.Content.ReadAsStringAsync();
            var respuesta = JsonConvert.DeserializeObject<List<OfertaDto>>(content);
            return View("Index",respuesta);
            

            
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CrearoofertaDto oferta)
        {
            var json=JsonConvert.SerializeObject(oferta);
            var content=new StringContent(json,Encoding.UTF8,"application/json");
           var ofertas= await _httpClient.PostAsync("api/Ofertas/CrearOferta",content);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            var response = await _httpClient.GetAsync($"api/Ofertas/Obtener/{Id}");
            var content=await response.Content.ReadAsStringAsync();
            var respuesta=JsonConvert.DeserializeObject<OfertaDto>(content);
            return View(respuesta);

        }
        [HttpPost]
        public async Task<IActionResult>Edit(OfertaDto oferta)
        {
            var json=JsonConvert.SerializeObject(oferta);
            var conten = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PutAsync("api/Ofertas/Editar", conten);
            return RedirectToAction("Index");

        }
    }
}
