using ConsumirapideEcomerceee.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Collections;
using System.Net.Http.Json;
namespace ConsumirapideEcomerceee.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly HttpClient _httpClient;
        public CategoriaController(IHttpClientFactory http)
        {
            _httpClient = http.CreateClient("MiApi");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response =await _httpClient.GetAsync("api/Categoria/Listar");
           
          
            var content=await response.Content.ReadAsStringAsync();
            var respuesta = JsonConvert.DeserializeObject<List<EditarCategoriaDto>>(content);
            List<EditarCategoriaDto> categorias = respuesta;
            return View("Index", respuesta);

        }
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CrearCategoriaDto categoria)
        {
            if (ModelState.IsValid)
            {
                var json=JsonConvert.SerializeObject(categoria);
                var content=new StringContent(json,Encoding.UTF8,"application/json");
                var response = await _httpClient.PostAsync("/api/Categoria/Crear", content);
            }
           return RedirectToAction("Index");

        }
    }
}
