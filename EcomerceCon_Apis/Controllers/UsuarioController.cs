using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Producto;
using EcomerceCon_Apis.Dtos.Usuarios;
using EcomerceCon_Apis.Models;
using EcomerceCon_Apis.Services;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IUsuarioService usuario;
        public UsuarioController(UserManager<Usuario> userManager,ProductosDbcontext contex, IUsuarioService usuarioService)
        {
            _userManager = userManager;
            _contex = contex;
            usuario = usuarioService;
        }
        [HttpPost]
        [Route("Registrar")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<CrearUsuarioDto>), StatusCodes.Status200OK)]

        public async Task<IActionResult> RegistrarUsuario([FromBody]CrearUsuarioDto request)
        {
            var resultado = await usuario.CrearUsuario(request);
            if (resultado.IsSuccess)
            {
                return Ok(resultado);
            }
            else
            {
                return BadRequest(resultado);
            }
        }

        [HttpGet("Perfil")]
        [Authorize]
        public async Task<IActionResult> ObtenerPerfil()
        {
            var usuarioId = _userManager.GetUserId(User);
            if (usuarioId == null)
            {
                return Unauthorized();
            }

            var datosUsuario = await usuario.ObtenerDatosUsuario(usuarioId);
            if (datosUsuario == null)
            {
                return NotFound();
            }

            return Ok(datosUsuario);
        }


    }
}
