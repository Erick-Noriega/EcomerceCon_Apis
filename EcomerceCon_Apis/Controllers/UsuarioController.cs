using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Producto;
using EcomerceCon_Apis.Dtos.Usuarios;
using EcomerceCon_Apis.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcomerceCon_Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ProductosDbcontext _contex;
        public UsuarioController(UserManager<Usuario> userManager,ProductosDbcontext contex)
        {
            _userManager = userManager;
            _contex = contex;
        }
        [HttpPost]
        [Route("Registrar")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<CrearUsuarioDto>), StatusCodes.Status200OK)]

        public async Task<IActionResult> RegistrarUsuario([FromBody]CrearUsuarioDto request)
        {
            var usuario = new Usuario
            {
            
                UserName = request.Correo,
                Email = request.Correo,
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Fecha_nac = request.Fecha_nac
            };
            if(usuario!=null && await _userManager.FindByEmailAsync(request.Correo) == null)
            {
                    await _userManager.CreateAsync(usuario, request.Password);


                var nuevocarrito = new Carrito
                {
                    UsuarioId = usuario.Id,
                    Fecha = DateTime.Now
                };
                _contex.Add(nuevocarrito);
                await _contex.SaveChangesAsync();
                _contex.UserRoles.Add(new IdentityUserRole<string> { UserId = usuario.Id, RoleId = "2" });
                await _contex.SaveChangesAsync();

                return Ok(new { mensaje = "Usuario registrado correctamente y se creo el carrito" });
            }
            else if(await _userManager.FindByEmailAsync(request.Correo)!=null)
            {
                return BadRequest(new { mensaje = "El correo ya está registrado" });
            }
            else {return BadRequest(new { mensaje = "Error al registrar el usuario" });}


        }


    }
}
