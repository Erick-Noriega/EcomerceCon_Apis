using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.DetalleCarrito;
using EcomerceCon_Apis.Dtos.Pedido;
using EcomerceCon_Apis.Dtos.Producto;
using EcomerceCon_Apis.Migrations;
using EcomerceCon_Apis.Models;
using EcomerceCon_Apis.Services;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
namespace EcomerceCon_Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class webhookController : ControllerBase
    {
        private readonly UserManager<Usuario> userManager;
        private readonly string _secretkey;
        private readonly IProductosService _productos;
        private readonly IWhatsappService _whattsapservices;
        private readonly IMemoryCache _memoryCache;
        private readonly IPedidoService _pedido;
        private ProductosDbcontext _context;
        private readonly IUsuarioService _usuarioService;
        public webhookController(IOptions<JwtSettings> conf,IPedidoService pedido, ProductosDbcontext context, UserManager<Usuario> u, IMemoryCache i,IWhatsappService W, IProductosService productos,IUsuarioService usuarioService )
        {
            userManager = u;
            _context = context;
            _secretkey = conf.Value.SecretKey;
            _memoryCache = i;
            _pedido = pedido;
            _whattsapservices = W;
            _productos = productos;
            _usuarioService = usuarioService;
        }



        [HttpGet("webhook")]
        public IActionResult Verificarwebhook(
    [FromQuery(Name = "hub.mode")] string mode,
    [FromQuery(Name = "hub.verify_token")] string token,
    [FromQuery(Name = "hub.challenge")] string challenge)
        {
            string verificacion = "EstaEsParawpp";

            if (mode == "subscribe" && token == verificacion)
            {
                return Content(challenge, "text/plain");
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpPost("webhook")]
        public async Task<IActionResult> RecibirMensaje([FromBody] JsonElement body)
        {
            if (body.TryGetProperty("entry", out var entry) &&
          entry.GetArrayLength() > 0 &&
          entry[0].TryGetProperty("changes", out var changes) &&
          changes.GetArrayLength() > 0 &&
          changes[0].TryGetProperty("value", out var value) &&
          value.TryGetProperty("messages", out var messages) &&
          messages.GetArrayLength() > 0)
            {
                
     
                var mensobjet = messages[0];
                string whatsappId = mensobjet.GetProperty("id").GetString();

                var mensajes = messages[0];

                string MensajeTexto = mensajes.GetProperty("text").GetProperty("body").GetString().Trim();
                string NumeroRemitente = mensajes.GetProperty("from").GetString();

                string mensajeRespuesta = "";
                string claveestado = $"estado_Login_{NumeroRemitente}";
                string claveparatiempo = $"tiempo_{NumeroRemitente}";
                string claveparamensajes = $"mensajes_{NumeroRemitente}";
                string claveTokenWhatsapp = $"token_whatsapp_{NumeroRemitente}";
                bool ms = false;

                if (_memoryCache.TryGetValue(claveTokenWhatsapp, out string tokenWhatsapp))
                {
                    try
                    {
                        HttpContext.User = ValidarTokenWhatsapp(tokenWhatsapp);

                        if (_memoryCache.TryGetValue(claveparatiempo, out DateTime ultimaActividadSesion) &&
                            DateTime.UtcNow - ultimaActividadSesion > TimeSpan.FromMinutes(5))
                        {
                            _memoryCache.Remove(claveTokenWhatsapp);
                            _memoryCache.Remove(claveestado);
                            _memoryCache.Remove(claveparatiempo);
                            _memoryCache.Remove($"destino_login_{NumeroRemitente}");

                            await _whattsapservices.EnviarRespuesta(
                                NumeroRemitente,
                                "⏰🔒 Tu sesión se cerró por inactividad durante más de 5 minutos.\n\nEscribí *2* para hacer un pedido o *3* para ver tu carrito.");
                            return Ok();
                        }

                        _memoryCache.Set(claveparatiempo, DateTime.UtcNow, TimeSpan.FromMinutes(5));
                    }
                    catch (SecurityTokenException)
                    {
                        _memoryCache.Remove(claveTokenWhatsapp);
                        _memoryCache.Remove(claveestado);
                        _memoryCache.Remove(claveparatiempo);
                        _memoryCache.Remove($"destino_login_{NumeroRemitente}");
                        await _whattsapservices.EnviarRespuesta(NumeroRemitente, "Tu sesión de WhatsApp expiró. Escribí *2* para hacer un pedido o *3* para ver tu carrito.");
                        return Ok();
                    }
                }

                try
                {
                    var registroMensaje = new MensajeRecibido
                    {
                        Mensaje = whatsappId,
                        fecha = DateTime.UtcNow

                    };

                    _context.MensajeRecibidos.Add(registroMensaje);
                    await _context.SaveChangesAsync();


                }
                catch (DbUpdateException)
                {

                    return Ok();
                }
            
                if (_memoryCache.TryGetValue($"mensajes_{NumeroRemitente}", out string mensaje) != null)
                {
                    ms = true;
                }

                if (_memoryCache.TryGetValue(claveestado, out string EstadoActual) && ms == true)
                {
                    _memoryCache.Set(claveparamensajes, "RECIBIO_MENSAJES", TimeSpan.FromMinutes(3));

                    if (EstadoActual != null)
                    {
                        if (_memoryCache.TryGetValue(claveparatiempo, out DateTime ultimaInteraccion))
                        {

                            TimeSpan tiempoTranscurrido = DateTime.UtcNow - ultimaInteraccion;


                            if (tiempoTranscurrido.TotalMinutes > 5)
                            {

                                _memoryCache.Remove(claveestado);
                                _memoryCache.Remove(claveparatiempo);
                                _memoryCache.Remove($"tiempo_{NumeroRemitente}");

                                await _whattsapservices.EnviarRespuesta(NumeroRemitente, "⏳ Tu sesión expiró por inactividad. Por favor, escribe *'Menu'* para comenzar de nuevo.");
                                return Ok();
                            }

                        }

                    }

                    if (EstadoActual == "ESPERANDO_CLAVE_RANDOM")
                    {
                        var resultadoLogin = await _usuarioService.IniciarSesionConClaveRandom(MensajeTexto);
                        if (!resultadoLogin.IsSuccess || resultadoLogin.Data == null)
                        {
                            _memoryCache.Set(claveestado, "ESPERANDO_CLAVE_RANDOM", TimeSpan.FromMinutes(5));
                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, "La clave o documento no fue reconocido. Inténtalo nuevamente.");
                            return Ok();
                        }

                        HttpContext.User = ValidarTokenWhatsapp(resultadoLogin.Data.Token);
                        var tiempoSesion = TimeSpan.FromMinutes(10);
                        _memoryCache.Set(claveTokenWhatsapp, resultadoLogin.Data.Token, tiempoSesion);
                        _memoryCache.Set(claveparatiempo, DateTime.UtcNow, tiempoSesion);

                        var destino = _memoryCache.Get<string>($"destino_login_{NumeroRemitente}");
                        _memoryCache.Remove($"destino_login_{NumeroRemitente}");
                        if (destino == "CARRITO")
                        {
                            mensajeRespuesta = await verCarrito(resultadoLogin.Data.Correo);
                            if (mensajeRespuesta.Contains("carrito está vacío", StringComparison.OrdinalIgnoreCase))
                            {
                                _memoryCache.Set(claveestado, "CARRITO_VACIO", tiempoSesion);
                                mensajeRespuesta += "\n¿Deseas agregar productos? Escribí *COMPRAR*.\nEscribí *CERRAR SESION* para cerrar sesión.";
                            }
                            else
                            {
                                _memoryCache.Set(claveestado, "USUARIO_LOGUEADO", tiempoSesion);
                                mensajeRespuesta += "\n🛒 ¿Desea finalizar su compra?\n\nEscriba PAGAR para comprar los productos.\nEscriba CANCELAR para cancelar sin vaciar el carrito.";
                            }
                        }
                        else if (destino == "PAGO")
                        {
                            _memoryCache.Set(claveestado, "ESPERANDOPEDIDO", tiempoSesion);
                            mensajeRespuesta = "Sesión iniciada. Por favor, ingresa el nombre de tu calle.";
                        }
                        else
                        {
                            _memoryCache.Set(claveestado, "ESPERANDO_NUMERO_PRODUCTO", tiempoSesion);
                            mensajeRespuesta = "✅ Sesión iniciada correctamente.\n" + await Productos() + "\nIndica el producto que quieres (escribe el número).";
                        }
                        await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);
                        return Ok();
                    }
                    else if (EstadoActual == "ESPERANDO_NUMERO_PRODUCTO")
                    {
                        string texto = MensajeTexto.Trim().ToUpperInvariant();

                        if (texto == "NO")
                        {
                            _memoryCache.Remove(claveestado);
                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, "Compra cancelada. Escribí *MENU* para ver las opciones disponibles.");
                            return Ok();
                        }

                        if(int.TryParse(MensajeTexto,out int numero)&& numero > 0)
                        {
                            var productosDisponibles = await _productos.lista();
                            if (!productosDisponibles.Any(producto => producto.Id == numero))
                            {
                                await _whattsapservices.EnviarRespuesta(NumeroRemitente, "Por favor, ingresa el número de un producto de la lista o escribí *NO* para cancelar.");
                                return Ok();
                            }

                            _memoryCache.Set($"ID_DE_PRODU_{NumeroRemitente}", MensajeTexto, TimeSpan.FromMinutes(5));
                            _memoryCache.Set(claveestado, "ESPERANDO_CANTIDAD", TimeSpan.FromMinutes(5));
                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, "🛒 Ingrese la cantidad que desea 🔢");
                            return Ok();
                        }
                        else
                        {
                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, "Por favor, ingresa el número de un producto de la lista o escribí *NO* para cancelar.");
                            return Ok();

                        }
                    }
                    else if (EstadoActual == "ESPERANDO_CANTIDAD")
                    {
                        if (int.TryParse(MensajeTexto, out int numero) && numero > 0) {

                            _memoryCache.Set($"CANTIDAD_DE_PRODU{NumeroRemitente}", MensajeTexto, TimeSpan.FromMinutes(5));
                            string id = "";
                            var emaila = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                            _memoryCache.TryGetValue($"ID_DE_PRODU_{NumeroRemitente}", out string a);

                            id = a;

                            CrearDetalleCarritoDto dto = new CrearDetalleCarritoDto
                            {
                                ProductoId = int.Parse(id),
                                cantidad = int.Parse(MensajeTexto.ToString())
                            };
                            var aux = await CrearDetalleCarrito(dto, emaila);
                            if (aux != null)
                            {
                                _memoryCache.Set(claveestado, "ESPERANDO_DES", TimeSpan.FromMinutes(5));
                                mensajeRespuesta = "🛒 ¡Producto agregado a tu carrito exitosamente! ✨";
                                await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);
                                mensajeRespuesta = "🛒 ¿Deseás seguir comprando o volver al menú?\n\n• Escribí MENU para volver al menú.\n• Escribí COMPRAR para seguir comprando.";
                                await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);




                                return Ok();
                            }

                        }
                        else
                        {
                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, "Por favor, ingrese un número válido.");
                            return Ok();
                        }
                            

                    }
                    else if(EstadoActual== "ESPERANDO_DES")
                    {
                        string txt = MensajeTexto.Trim().ToUpperInvariant();
                        if (txt == "COMPRAR")
                        {
                            _memoryCache.Set(claveestado, "ESPERANDO_NUMERO_PRODUCTO");

                            mensajeRespuesta = await Productos() + "\n" + " Indique el producto que quiere (poniendo el numero de al lado)";
                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);

                            return Ok();
                        }

                        if (txt == "MENU")
                        {
                            _memoryCache.Remove(claveestado);
                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, "👋 Menú principal:\n1. Productos\n2. Hacer un Pedido\n3. Ver Carrito");
                            return Ok();
                        }

                        await _whattsapservices.EnviarRespuesta(NumeroRemitente, "Por favor, ingresa una de estas dos opciones: *MENU* o *COMPRAR*.");
                        return Ok();
                    }

                        else if (EstadoActual == "CARRITO_VACIO")
                        {
                            string textoRecibido = MensajeTexto.Trim().ToUpperInvariant();

                            if (textoRecibido == "COMPRAR")
                            {
                            var emailSesion = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                                if (string.IsNullOrWhiteSpace(emailSesion))
                                {
                                    _memoryCache.Set($"destino_login_{NumeroRemitente}", "PEDIDO", TimeSpan.FromMinutes(5));
                                    _memoryCache.Set(claveestado, "ESPERANDO_CLAVE_RANDOM", TimeSpan.FromMinutes(5));
                                    await _whattsapservices.EnviarRespuesta(NumeroRemitente, "Para agregar productos, escribe tu clave aleatoria/documento:");
                                    return Ok();
                                }

                                _memoryCache.Set(claveestado, "ESPERANDO_NUMERO_PRODUCTO", TimeSpan.FromMinutes(6));
                                mensajeRespuesta = await Productos() + "\n Indique el producto que quiere (poniendo el numero de al lado)";
                                await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);
                                return Ok();
                            }

                            if (textoRecibido is "CERRAR SESION" or "CERRAR SESIÓN")
                            {
                                _memoryCache.Remove(claveestado);
                                _memoryCache.Remove(claveparatiempo);
                                _memoryCache.Remove(claveTokenWhatsapp);
                                await _whattsapservices.EnviarRespuesta(NumeroRemitente, "Sesión cerrada. Escribí *MENU* para volver a comenzar.");
                                return Ok();
                            }

                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, "Por favor, escribe una de estas opciones: *COMPRAR* o *CERRAR SESION*.");
                            return Ok();
                        }
                        else if (EstadoActual == "USUARIO_LOGUEADO")
                        {
                            string textoRecibido = MensajeTexto.Trim().ToUpperInvariant();

                            if (textoRecibido == "PAGAR")
                            {
                                var emailSesion = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                                if (string.IsNullOrWhiteSpace(emailSesion))
                                {
                                    _memoryCache.Set($"destino_login_{NumeroRemitente}", "CARRITO", TimeSpan.FromMinutes(5));
                                    _memoryCache.Set(claveestado, "ESPERANDO_CLAVE_RANDOM", TimeSpan.FromMinutes(5));
                                    await _whattsapservices.EnviarRespuesta(NumeroRemitente, "Para continuar con el pedido, escribe tu clave aleatoria/documento:");
                                    return Ok();
                                }

                                _memoryCache.Set(claveestado, "ESPERANDOPEDIDO", TimeSpan.FromMinutes(5));
                                await _whattsapservices.EnviarRespuesta(NumeroRemitente, "por favor ingresa el nombre de tu calle");
                                return Ok();
                            }

                            if (textoRecibido == "CANCELAR")
                            {
                                _memoryCache.Remove(claveestado);
                                await _whattsapservices.EnviarRespuesta(NumeroRemitente, "Operación cancelada. Tu carrito se mantiene intacto. Escribí *MENU* para ver las opciones disponibles.");
                                return Ok();
                            }

                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, "Por favor, ingresa una de estas dos opciones: *PAGAR* o *CANCELAR*.");
                            return Ok();

                        }
                        else if (EstadoActual == "ESPERANDOPEDIDO")
                        {
                            string textoRecibido = MensajeTexto.ToUpper();


                            _memoryCache.Set($"NOMBRE_DE_CALLE_{NumeroRemitente}", textoRecibido, TimeSpan.FromMinutes(5));

                            _memoryCache.Set(claveestado, "ESPERANDO_NUMERO", TimeSpan.FromMinutes(5));
                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, "📍 Por favor, ingresa tu localidad. 🏡");
                            return Ok();



                        }
                        else if (EstadoActual == "ESPERANDO_NUMERO")
                        {
                            string textoRecibido = MensajeTexto.ToUpper();

                            _memoryCache.Set($"NUMERO_DE_CALLE_{NumeroRemitente}", textoRecibido, TimeSpan.FromMinutes(5));
                            _memoryCache.Set(claveestado, "ESPERANDO_PROVINCIA", TimeSpan.FromMinutes(5));
                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, "🗺️ Por favor, ingresa tu provincia. 📍");
                            return Ok();


                        }
                        else if (EstadoActual == "ESPERANDO_PROVINCIA")
                        {
                            string textoRecibido = MensajeTexto.ToUpper();
                            ;
                            _memoryCache.Set($"ESPERANDO_LOCALIDAD{NumeroRemitente}", textoRecibido, TimeSpan.FromMinutes(5));
                            _memoryCache.Set(claveestado, "ESPERANDO_LOCALIDAD", TimeSpan.FromMinutes(5));
                            return Ok();
                        }
                        else if (EstadoActual == "ESPERANDO_LOCALIDAD")
                        {
                            string textoRecibido = MensajeTexto.ToUpper();
                            _memoryCache.TryGetValue($"ESPERANDO_LOCALIDAD{NumeroRemitente}", out string localidadDeUsuario);
                            string calle = "";
                            string numero = "";
                            string provincia = "";
                            if (
                               _memoryCache.TryGetValue($"NOMBRE_DE_CALLE_{NumeroRemitente}", out string callee)
                                )
                            {
                                calle = callee;
                            }
                            if (_memoryCache.TryGetValue($"ESPERANDO_LOCALIDAD{NumeroRemitente}", out string provinciaa))
                            {
                                provincia = provinciaa;

                            }
                            if (_memoryCache.TryGetValue($"NUMERO_DE_CALLE_{NumeroRemitente}", out string num))
                            {
                                numero = num;

                            }

                            DireccionDeEnvioDto direccion = new DireccionDeEnvioDto
                            {
                                calle = calle,
                                provincia = provincia,
                                Localidad = numero


                            };
                            var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                            if (!string.IsNullOrWhiteSpace(email))
                            {
                                mensajeRespuesta = await ConfirmarPedido(direccion, email, NumeroRemitente);
                                await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);

                                return Ok();
                            }
                        }
                    }
                

                    if (MensajeTexto == "1")
                    {
                        mensajeRespuesta = await Productos();
                    }
                    else if (MensajeTexto == "2")
                    {
                        var emailLogueado = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                        if (string.IsNullOrWhiteSpace(emailLogueado))
                        {
                            _memoryCache.Set($"destino_login_{NumeroRemitente}", "PEDIDO", TimeSpan.FromMinutes(5));
                            _memoryCache.Set(claveestado, "ESPERANDO_CLAVE_RANDOM", TimeSpan.FromMinutes(5));
                            _memoryCache.Set(claveparatiempo, DateTime.UtcNow, TimeSpan.FromMinutes(5));
                            mensajeRespuesta = "Para hacer tu pedido, escribe tu clave aleatoria/documento:";
                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);
                            return Ok();
                        }

                        _memoryCache.Set(claveestado, "ESPERANDO_NUMERO_PRODUCTO", TimeSpan.FromMinutes(10));
                        mensajeRespuesta = "Ya estás autenticado. " + await Productos() + "\nIndica el producto que quieres (escribe el número).";
                        await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);
                        return Ok();
                    }
                    else if (MensajeTexto == "3")
                    {
                        var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

                        if (string.IsNullOrWhiteSpace(email))
                        {
                            _memoryCache.Set($"destino_login_{NumeroRemitente}", "CARRITO", TimeSpan.FromMinutes(5));
                            _memoryCache.Set(claveestado, "ESPERANDO_CLAVE_RANDOM", TimeSpan.FromMinutes(5));
                            _memoryCache.Set(claveparatiempo, DateTime.UtcNow, TimeSpan.FromMinutes(5));
                            mensajeRespuesta = "Para ver tu carrito, escribe tu clave aleatoria/documento:";
                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);
                            return Ok();
                        }

                        mensajeRespuesta = await verCarrito(email);

                        if (mensajeRespuesta.Contains("carrito está vacío", StringComparison.OrdinalIgnoreCase))
                        {
                            _memoryCache.Set(claveestado, "CARRITO_VACIO", TimeSpan.FromMinutes(6));
                            mensajeRespuesta += "\n¿Deseas agregar productos? Escribí *COMPRAR*.\nEscribí *CERRAR SESION* para cerrar sesión.";
                        }
                        else
                        {
                            _memoryCache.Set(claveestado, "USUARIO_LOGUEADO", TimeSpan.FromMinutes(6));
                            mensajeRespuesta += "\n🛒 ¿Desea finalizar su compra?\n\nEscriba PAGAR para comprar los productos.\nEscriba CANCELAR para cancelar sin vaciar el carrito.";
                        }

                        await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);
                        return Ok();
                    }
                    else
                    {
                        _memoryCache.Set(claveparamensajes, "RECIBIO_MENSAJES", TimeSpan.FromMinutes(3));
                        mensajeRespuesta = "👋 Bienvenido a nuestra tienda. Por favor, elija una opción:\n1. Productos\n2. Hacer un Pedido\n3. Ver Carrito";
                    }

                    Console.WriteLine($"Mensaje recibido de {NumeroRemitente}: {MensajeTexto}");
                
                    await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);
                

            }
                return Ok();
        }

        private ClaimsPrincipal ValidarTokenWhatsapp(string token)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretkey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out _);
        }

        private async Task<string> verCarrito(string email, string clave="")
        {


            var usuario = await userManager.FindByEmailAsync(email);
            if (usuario == null)
            {
                return "No se encontró el usuario asociado al carrito.";
            }

            var usuarioId = usuario.Id;
            var ca = await _context.Carritos.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
            if (ca == null)
            {
                return "🛒 Tu carrito está vacío.";
            }

            var carrito = await _context.DetalleCarritos.Where(c => c.CarritoId == ca.Id).Select(c => new DetalleCarritoDto
            {
                Id = c.Id,
                ProductoId = c.ProductoId,
                NombreProducto = c.producto.Nombre,
                ImagenUrl = c.producto.ImagenUrl,
                PrecioUnitario = c.PrecioPorUnidad,
                Cantidad = c.cantidad,

            })
                .ToListAsync();
            if (carrito.Count == 0)
            {
                return "🛒 Tu carrito está vacío.";
            }

            string mostrarCarrito = "tus productos son:\n\n";

            foreach (var item in carrito)
            {
                mostrarCarrito += $"Nombre:{item.NombreProducto}\n";
                mostrarCarrito += "Precio por unidad " + item.PrecioUnitario.ToString() + "\n";
                mostrarCarrito += "Cantidad " + item.Cantidad.ToString() + "\n";
            }
            return mostrarCarrito;

        }
        private async Task<string> Productos()
        {

            var listaproductos = await _productos.lista();
            string productos = "nuestros productos son:\n\n";

        foreach (var item in listaproductos)
            {
                productos += $"🔹 *{item.Id}.* {item.Nombre} — *${item.PrecioFinal}*\n";
            }
            productos += "\n🛍️ **¿Cual quieres comprar?**\n• Escribí *NO* para cancelar";
            return productos;
        }

        private async Task<string> ConfirmarPedido([FromBody] DireccionDeEnvioDto direccion, string email, string numero)
        {
            var userId = await _usuarioService.ObtenerIdUsuario(email);
            if (userId == null)
            {
                return "Usuario no autenticado.";
            }

            var usuario = await userManager.FindByIdAsync(userId);
            if (usuario == null)
            {
                return "Usuario no autenticado.";
            }

            usuario.Numero_telefono = numero;
            var actualizacionUsuario = await userManager.UpdateAsync(usuario);
            if (!actualizacionUsuario.Succeeded)
            {
                return "No se pudo guardar el número de WhatsApp para las notificaciones del pedido.";
            }

            string respuesta = "";
            var linkdepago=await _pedido.AgregarPedido(direccion, userId);
            if (linkdepago.Exito)
            {
                respuesta = linkdepago.InitPoint;
            }
            return respuesta;


        }

        private async Task<string> CrearDetalleCarrito([FromBody] CrearDetalleCarritoDto crearDetalleCarritoDto,string email)
        {
            try
            {
                var fecha = DateTime.Now;
                var productoDb = await _context.Productos.Include(o => o.ofertas).Where(p => p.Id == crearDetalleCarritoDto.ProductoId)
                    .FirstOrDefaultAsync();

                List<ProductoDto> producto = await _context.Productos.Include(c => c.categoria)
                     .Include(o => o.ofertas).Where(p => p.Id == crearDetalleCarritoDto.ProductoId).Select(p => new ProductoDto
                     {

                         PrecioFinal = p.ofertas.Any(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin)
 ? p.precio - (p.precio * (p.ofertas
     .Where(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin)
     .Select(op => op.oferta.MontoAdescontar)
     .FirstOrDefault() / 100)) // <-- Usa 100.0 si tus precios son double/float
 : p.precio
                     })
                     .ToListAsync();

                var precioFinal = producto.Select(p => p.PrecioFinal).FirstOrDefault();

           
                var usuario = await userManager.FindByEmailAsync(email) ?? await userManager.FindByEmailAsync(email);
                var usuarioId = usuario.Id;
                var carrito = await _context.Carritos.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
                if (carrito == null)
                {
                    return"El Usuario No tiene Carrito";
                }
                var detalleExistente = await _context.DetalleCarritos.FirstOrDefaultAsync(d => d.CarritoId == carrito.Id && d.ProductoId == crearDetalleCarritoDto.ProductoId);

                if (detalleExistente != null)
                {
                    detalleExistente.cantidad += crearDetalleCarritoDto.cantidad;
                    if (detalleExistente.cantidad <= 0)
                    {
                        _context.DetalleCarritos.Remove(detalleExistente);
                        await _context.SaveChangesAsync();
                        return "producto removido del carrito";
                    }
                    else
                    {

                        _context.DetalleCarritos.Update(detalleExistente);
                    }
                    detalleExistente.PrecioPorUnidad = precioFinal;
                }
                else
                {
                    var NuevoDetalle = new DetalleCarrito
                    {
                        CarritoId = carrito.Id,
                        ProductoId = crearDetalleCarritoDto.ProductoId,
                        cantidad = crearDetalleCarritoDto.cantidad,
                        PrecioPorUnidad = precioFinal
                    };
                    _context.DetalleCarritos.Add(NuevoDetalle);

                }
                await _context.SaveChangesAsync();
                return "Producto agregado correctamente al carrito.";
            }
            catch (Exception ex)
            {

                return  "error " + ex.Message.ToString();
            }




        }
    }
    }


    

