using ConsumirapideEcomerceee.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
namespace ConsumirapideEcomerceee.Controllers
{
    public class ProductoController : Controller
    {
        private readonly HttpClient _httpClient;
        public ProductoController(IHttpClientFactory htpp)
        {
            _httpClient=htpp.CreateClient("MiApi");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task <IActionResult> Index()
        {
            var token = User.FindFirstValue("JWToKen");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("api/Producto/ListaParaAdmins");
            var json=await response.Content.ReadAsStringAsync();
            var productos = JsonConvert.DeserializeObject<List<ProductoDtoParaAdmins>>(json);
         
            return View(productos);
        
            
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var response = await _httpClient.GetAsync("api/Categoria/Listar");
            var conten=await response.Content.ReadAsStringAsync();
            var categorias = JsonConvert.DeserializeObject<List<EditarCategoriaDto>>(conten);
            ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre");
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Create(CrearProductoDto prod)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(prod);
                var content =  new StringContent(json, Encoding.UTF8, "application/json");
                await _httpClient.PostAsync("api/Producto/Crear", content);
                return RedirectToAction("Index");

            }
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Categoria/Listar");
                var conten = await response.Content.ReadAsStringAsync();
                var categorias = JsonConvert.DeserializeObject<List<EditarCategoriaDto>>(conten);
                ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre");
                var json = await _httpClient.GetAsync($"api/Producto/Obtener/{Id}");
                var content = await json.Content.ReadAsStringAsync();
                var respuesta = JsonConvert.DeserializeObject<ResultadoApi<EditarProductoDto>>(content);
                var produ = respuesta.Data;
                return View("Editar", produ);

            }
            catch (Exception)
            {

                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public async Task<IActionResult>Edit(EditarProductoDto dto)
        {
            var json= JsonConvert.SerializeObject(dto);
            var cont=new StringContent(json, Encoding.UTF8,"application/json");
            await _httpClient.PutAsync("api/Producto/Editar", cont);
            return RedirectToAction("Index");

        }
    }
}
