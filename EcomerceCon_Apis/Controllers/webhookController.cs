using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.DetalleCarrito;
using EcomerceCon_Apis.Dtos.Login;
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
        private readonly IWhatsappService _whattsapservices;
        private readonly IMemoryCache _memoryCache;
        private ProductosDbcontext _context;
        public webhookController(IOptions<JwtSettings> conf, ProductosDbcontext context, UserManager<Usuario> u, IMemoryCache i,IWhatsappService W)
        {
            userManager = u;
            _context = context;
            _secretkey = conf.Value.SecretKey;
            _memoryCache = i;
            _whattsapservices = W;
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
                string claveparacarrito = $"estado_Carrito{NumeroRemitente}";
                bool ms = false;

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

                string EstadoCarrito = "";
                if (_memoryCache.TryGetValue(claveestado, out string EstadoActual) && ms == true || _memoryCache.TryGetValue(claveparacarrito, out EstadoCarrito))
                {
                    _memoryCache.Set(claveparamensajes, "RECIBIO_MENSAJES", TimeSpan.FromMinutes(3));

                    _memoryCache.TryGetValue(claveestado, out string estadoactual);
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

                    else if (EstadoCarrito == "ESPERANDO_EMAILPARA_PEDIDO")
                    {
                        _memoryCache.Set($"Email_Carrito_{NumeroRemitente}", MensajeTexto, TimeSpan.FromMinutes(5));
                        _memoryCache.Set(claveparacarrito, "ESPERANDO_CONTRA", TimeSpan.FromMinutes(5));
                        mensajeRespuesta = "Ahora Ingresa tu contraseñaaaaaaaaaaaaaa:";
                        await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);
                        return Ok();
                    }
                    else if (EstadoCarrito == "ESPERANDO_CONTRA")
                    {
                        _memoryCache.TryGetValue($"Email_Carrito_{NumeroRemitente}", out string emailcarrito);
                        _memoryCache.Set($"Contra_Carrito{NumeroRemitente}", MensajeTexto, TimeSpan.FromMinutes(5));
                        _memoryCache.Set(claveparacarrito, "USUARIO_LOG_CARRITO", TimeSpan.FromMinutes(5));
                        var log = await Loguearse(MensajeTexto, emailcarrito);
                        if (log != null)
                        {
                            mensajeRespuesta = "Logueado correctamente ...elija los productos que quiera comprar /Si desea comprar Escriba COMPRAR";
                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);
                            return Ok();
                        }
                    }

                    else if (EstadoCarrito == "USUARIO_LOG_CARRITO")
                    {
                        string texto = MensajeTexto.ToUpper();
                        if (texto == "COMPRAR")
                        {
                            mensajeRespuesta = await Productos() + "\n" + " Indique el producto que quiere (poniendo el numero de al lado)";
                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);
                        }
                        _memoryCache.Set(claveparacarrito, "ESPERANDO_NUMERO_PRODUCTO");
                        return Ok();

                    }

                    else if (EstadoCarrito == "ESPERANDO_NUMERO_PRODUCTO")
                    {
                        _memoryCache.Set($"ID_DE_PRODU_{NumeroRemitente}", MensajeTexto, TimeSpan.FromMinutes(5));
                        _memoryCache.Set(claveparacarrito, "ESPERANDO_CANTIDAD");
                        await _whattsapservices.EnviarRespuesta(NumeroRemitente, "ingrese la cantiad que desea");
                        return Ok();
                    }
                    else if (EstadoCarrito == "ESPERANDO_CANTIDAD")
                    {
                        _memoryCache.Set($"CANTIDAD_DE_PRODU{NumeroRemitente}", MensajeTexto, TimeSpan.FromMinutes(5));
                        string id = "";
                        string emaila = "";
                        if (_memoryCache.TryGetValue($"Email_Carrito_{NumeroRemitente}", out string p))
                        {
                            emaila = p;
                        }
                        if (_memoryCache.TryGetValue($"ID_DE_PRODU_{NumeroRemitente}", out string a))
                        {
                            id = a;
                        }
                        CrearDetalleCarritoDto dto = new CrearDetalleCarritoDto
                        {
                            ProductoId = int.Parse(id),
                            cantidad = int.Parse(MensajeTexto)
                        };
                        var aux = await CrearDetalleCarrito(dto, emaila);
                        if (aux != null)
                        {
                            mensajeRespuesta = "Producto agrega a tu carrito exitosamente";
                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);
                            return Ok();
                        }

                    }

                        if (EstadoActual == "ESPERANDO_EMAIL")
                        {
                            _memoryCache.Set($"temp_email_{NumeroRemitente}", MensajeTexto, TimeSpan.FromMinutes(5));
                            _memoryCache.Set(claveestado, "ESPERANDO_PASSWORD", TimeSpan.FromMinutes(5));

                            mensajeRespuesta = "Ahora ingresa tu contraseña:";
                            _memoryCache.Set(claveparatiempo, DateTime.UtcNow, TimeSpan.FromMinutes(5));
                            if (MensajeTexto != "3")
                            {
                                await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);
                            }


                            return Ok();
                        }



                        else if (EstadoActual == "ESPERANDO_PASSWORD")
                        {


                            _memoryCache.TryGetValue($"temp_email_{NumeroRemitente}", out string emailingresado);

                            var usuariovalido = await Loguearse(MensajeTexto, emailingresado);
                            if (usuariovalido != null)
                            {
                                _memoryCache.Set($"usuario_logueado_{NumeroRemitente}", emailingresado, TimeSpan.FromMinutes(6));
                                _memoryCache.Set(claveestado, "USUARIO_LOGUEADO", TimeSpan.FromMinutes(6));



                                mensajeRespuesta = await verCarrito(emailingresado, MensajeTexto);
                                if (!mensajeRespuesta.Contains("vacio"))
                                {
                                    mensajeRespuesta += "\n¿Desea vaciar el carrito (comprar los productos)?\nEscriba *PAGAR* si desea vaciar el carrito / Escriba *VACIAR* si quiere Cancelar";
                                }

                                await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);
                                return Ok();

                            }
                            else
                            {
                                _memoryCache.Remove(claveestado);
                                _memoryCache.Remove($"temp_email_{NumeroRemitente}");

                                mensajeRespuesta = "Correo electrónico o contraseña incorrectos. Escribe *3* para intentar de nuevo.";



                                await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);


                                return Ok();


                            }
                        }

                        else if (EstadoActual == "USUARIO_LOGUEADO")
                        {
                            string textoRecibido = MensajeTexto.ToUpper();
                            if (textoRecibido == "PAGAR")
                            {


                                _memoryCache.Set(claveestado, "ESPERANDOPEDIDO", TimeSpan.FromMinutes(5));
                                await _whattsapservices.EnviarRespuesta(NumeroRemitente, "por favor ingresa el nombre de tu calle");

                            }
                            return Ok();

                        }
                        else if (EstadoActual == "ESPERANDOPEDIDO")
                        {
                            string textoRecibido = MensajeTexto.ToUpper();


                            _memoryCache.Set($"NOMBRE_DE_CALLE_{NumeroRemitente}", textoRecibido, TimeSpan.FromMinutes(5));

                            _memoryCache.Set(claveestado, "ESPERANDO_NUMERO", TimeSpan.FromMinutes(5));
                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, "por favor ingresa tu localidad");
                            return Ok();



                        }
                        else if (EstadoActual == "ESPERANDO_NUMERO")
                        {
                            string textoRecibido = MensajeTexto.ToUpper();

                            _memoryCache.Set($"NUMERO_DE_CALLE_{NumeroRemitente}", textoRecibido, TimeSpan.FromMinutes(5));
                            _memoryCache.Set(claveestado, "ESPERANDO_PROVINCIA", TimeSpan.FromMinutes(5));
                            string auxa = $"temp_email_{NumeroRemitente}";
                            string e = "";
                            if (_memoryCache.TryGetValue(auxa, out string email))
                            {
                                e = email;
                            }
                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, "por favor ingresa tu provincia");
                            return Ok();


                        }
                        else if (EstadoActual == "ESPERANDO_PROVINCIA")
                        {
                            string textoRecibido = MensajeTexto.ToUpper();
                            ;
                            _memoryCache.Set($"ESPERANDO_LOCALIDAD{NumeroRemitente}", textoRecibido, TimeSpan.FromMinutes(5));
                            _memoryCache.Set(claveestado, "ESPERANDO_LOCALIDAD", TimeSpan.FromMinutes(5));

                            await _whattsapservices.EnviarRespuesta(NumeroRemitente, "por favor ingresa tu localidad");
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
                            if (_memoryCache.TryGetValue($"temp_email_{NumeroRemitente}", out string email))
                            {

                                mensajeRespuesta = await ConfirmarPedido(direccion, email);
                                _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);

                                return Ok();
                            }
                            if (_memoryCache.TryGetValue($"estadopago", out string m))
                            {
                                var ma = m;
                            }
                        }
                    }
                

                    if (MensajeTexto == "1")
                    {
                        mensajeRespuesta = await Productos();
                    }
                    else if (MensajeTexto == "2")
                    {
                        if (!(_memoryCache.TryGetValue($"USUARIO_LOG_CARRITO{NumeroRemitente}", out string emailLogueado)))

                        {
                            _memoryCache.Set(claveparacarrito, "ESPERANDO_EMAILPARA_PEDIDO", TimeSpan.FromMinutes(5));
                            mensajeRespuesta = "Para hacer tu pedido debes iniciar sesión.\nPor favor, ingresa tu *correo electrónico*:";
                        }

                    }
                    else if (MensajeTexto == "3")
                    {


                        if (!(_memoryCache.TryGetValue($"usuario_logueado_{NumeroRemitente}", out string emailLogueado)))
                        {

                            _memoryCache.Set(claveestado, "ESPERANDO_EMAIL", TimeSpan.FromMinutes(5));
                            mensajeRespuesta = "Para ver tu carrito debes iniciar sesión.\nPor favor, ingresa tu *correo electrónico*:";
                        }
                    }
                    else
                    {
                        _memoryCache.Set(claveparamensajes, "RECIBIO_MENSAJES", TimeSpan.FromMinutes(3));
                        mensajeRespuesta = "Bienvenido a nuestra tienda, por favor elija una opción:\n1. Productos\n2. Hacer un Pedido\n3. Ver Carrito";
                    }

                    Console.WriteLine($"Mensaje recibido de {NumeroRemitente}: {MensajeTexto}");
                
                    await _whattsapservices.EnviarRespuesta(NumeroRemitente, mensajeRespuesta);
                

            }
                return Ok();
        }

        private async Task<string> verCarrito(string email, string clave)
        {


            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;
            var ca = await _context.Carritos.FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);


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
            string mostrarCarrito = "tus productos son:\n\n";

            foreach (var item in carrito)
            {
                mostrarCarrito += $"Nombre:{item.NombreProducto}\n";
                mostrarCarrito += "Precio por unidad " + item.PrecioUnitario.ToString() + "\n";
                mostrarCarrito += "Cantidad " + item.Cantidad.ToString() + "\n";
            }
            // mostrarCarrito+= "¿Desea vaciar el carrito(comprar los productos)?/Escriba SI si desea vaciar el carrito / Escriba NO si quiere Cancelar";
            return mostrarCarrito;

        }


        private async Task<string> Productos()
        {
            var fecha = DateTime.Now;

            List<ProductoDto> listaproductos = await _context.Productos.Include(c => c.categoria)
                       .Include(o => o.ofertas).Select(p => new ProductoDto
                       {

                           Id = p.Id,
                           ImagenUrl = p.ImagenUrl,
                           Descripcion = p.Descripcion,
                           Nombre = p.Nombre,
                           CategoriaId = p.CategoriaId,
                           CategoriaNombre = p.categoria.Nombre,
                           PrecioOriginal = p.precio,
                           TieneOferta = p.ofertas.Any(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin),
                           PrecioFinal = p.ofertas.Any(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin)
   ? p.precio - (p.precio * (p.ofertas
       .Where(op => fecha >= op.oferta.Fecha_inicio && fecha <= op.oferta.Fecha_fin)
       .Select(op => op.oferta.MontoAdescontar)
       .FirstOrDefault() / 100)) // <-- Usa 100.0 si tus precios son double/float
   : p.precio
                       })
                       .ToListAsync();
            string productos = "nuestros productos son:\n\n";
            foreach (var item in listaproductos)
            {
                productos +=item.Id + item.Nombre + "-" + "$" + item.PrecioFinal.ToString() + "\n";
            }
            productos += "¿Quiere Comprar alguno? Escriba SI si desea comprar / Escriba NO si quiere Cancelar";
            return productos;
        }

  

        private async Task<IActionResult> Loguearse(string clave, string correo)
        {
            bool aux = false;
            var user = await userManager.FindByEmailAsync(correo);
            if (user != null)
            {
                aux = true;
            }
            bool EsCorrecta = await userManager.CheckPasswordAsync(user, clave);
            if (EsCorrecta == true && aux == true)
            {
                var keybytes = Encoding.UTF8.GetBytes(_secretkey);
                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, correo));
                var tokendescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keybytes), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokendescriptor);
                var tokencreado = tokenHandler.WriteToken(tokenConfig);
                var respuesta = new RespuestaGenericaDto<LoginRespondoDto>
                {
                    IsSuccess = true,
                    Mensaje = "usuario verificado correctamente",
                    Data = new LoginRespondoDto
                    {
                        Token = tokencreado,
                        Correo = correo
                    }
                };
                return Ok(respuesta);
            }
            else
            {
                var respuestaError = new RespuestaGenericaDto<LoginRespondoDto>
                {
                    IsSuccess = false,
                    Mensaje = "Usuario o contraseña incorrectos",
                    Data = null
                };
                return Unauthorized(respuestaError);
            }

        }
        private bool TryExtraerDatos(JsonElement body, out string telefono, out string mensaje)
        {
            telefono = string.Empty;
            mensaje = string.Empty;
            try
            {
                var entry = body.GetProperty("entry")[0];
                var changes = entry.GetProperty("changes")[0];
                var value = changes.GetProperty("value");

                if (value.TryGetProperty("messages", out JsonElement messages) && messages.GetArrayLength() > 0)
                {
                    var message = messages[0];
                    telefono = message.GetProperty("from").GetString() ?? string.Empty;
                    if (message.TryGetProperty("type", out JsonElement type) && type.GetString() == "text")
                    {
                        mensaje = message.GetProperty("text").GetProperty("body").GetString() ?? string.Empty;
                        return true;
                    }


                }



            }
            catch
            {

                return false;
            }
            return false;




        }
        private async Task<string> ConfirmarPedido([FromBody] DireccionDeEnvioDto direccion, string email)

        {

            var usuario = await userManager.FindByEmailAsync(email) ?? await userManager.FindByEmailAsync(email);
            if (usuario == null)
            {
                return "Usuario no autenticado.";
            }




            if (string.IsNullOrEmpty(usuario.Id))
            {
                return "Usuario no autenticado.";
            }

            var carrito = await _context.Carritos.Include(c => c.DetalleCarrito).ThenInclude(d => d.producto).FirstOrDefaultAsync(c => c.UsuarioId == usuario.Id);
            if (carrito == null || carrito.DetalleCarrito == null || !carrito.DetalleCarrito.Any())
            {
                return "Carrito no encontrado";
            }
            using var transaction = await _context.Database.BeginTransactionAsync();
            var pedido = new Pedido
            {

                UsuarioId = usuario.Id,
                fecha_pedido = DateTime.Now,
                total = (float)carrito.DetalleCarrito.Sum(d => d.PrecioPorUnidad * d.cantidad),
                estadoPedido = EstadoPedido.Pendiente,
                DireccionDeEnvio = direccion.DireccionCompleta,
                detallePedido = carrito.DetalleCarrito.Select(d => new DetallePedido
                {
                    ProductoId = d.ProductoId,
                    Cantidad = d.cantidad,
                    PrecioPorUni = d.PrecioPorUnidad,


                }).ToList(),

            };
            var estadoepdido = EstadoPedido.Pendiente;
            var comprobar = await _context.Pedidos
      .FirstOrDefaultAsync(p => p.UsuarioId == usuario.Id && p.estadoPedido == estadoepdido);

            if (comprobar == null)
            {

                _context.Pedidos.Add(pedido);

                await _context.SaveChangesAsync();
            }




            var client = new PreferenceClient();
            var prefencelient = new PreferenceRequest
            {
                ExternalReference = pedido.Id.ToString(),
                Items = carrito.DetalleCarrito.Select(item => new PreferenceItemRequest
                {
                    Title = item.producto!.Nombre,
                    Quantity = item.cantidad,
                    CurrencyId = "ARS",
                    UnitPrice = (decimal)item.PrecioPorUnidad
                }).ToList(),
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = "https://localhost:5023/Pedido/Pago_exitoso",
                    Failure = "http://localhost:5023/Pedido/Fallido",
                    Pending = "https://localhost:5023/Pedido/Pendiente"
                },


                NotificationUrl = "https://scarcity-growl-sectional.ngrok-free.dev/api/Pagos/RegistrarPago",

                AutoReturn = "approved",

               

            };
         


            Preference preference = await client.CreateAsync(prefencelient);
            await transaction.CommitAsync();

            


            return preference.InitPoint;

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


    

