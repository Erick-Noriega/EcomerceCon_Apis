using ConsumirapideEcomerceee.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ConsumirapideEcomerceee.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly HttpClient _httpClient;
        public UsuarioController(IHttpClientFactory htpp) 
        { 
            _httpClient=htpp.CreateClient("MiApi");
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registrar(CrearUsuarioDto usuario)
        {
            try
            {
                var json = JsonConvert.SerializeObject(usuario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await _httpClient.PostAsync("api/Usuario/Registrar", content);
                return RedirectToAction("Validar");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error al procesar la solicitud: " + ex.Message);
                return View(usuario);
            }
        }



        [HttpGet]
        public IActionResult Validar()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Validar(Credenciales credenciales)
        {
            try
            {

                var json = JsonConvert.SerializeObject(credenciales);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/Autenticacion/Login", content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonRespuesta = await response.Content.ReadAsStringAsync();

                    var resultado = JsonConvert.DeserializeObject<ResultadoApi<ResultadoCredencial>>(jsonRespuesta);

                    var tokenString = resultado.Data.token;
                    var role = resultado.Data.Role;

                    var handler = new JwtSecurityTokenHandler();
                    var jwtoken = handler.ReadJwtToken(tokenString);
                    var claims = jwtoken.Claims.ToList();

                    claims.Add(new Claim("JWToken", tokenString));

                    // Agregar el rol devuelto por la API
                    if (!string.IsNullOrWhiteSpace(role))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var claimsIdentity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        ClaimTypes.Name,
                        ClaimTypes.Role
                    );

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = false
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Credenciales incorrectas");
                return Unauthorized();


            }
            catch (Exception)
            {

                throw ;
            }

        }
        [HttpGet]
        public IActionResult Mostrador() 
        {
            return View  ();
        }



    }
}
