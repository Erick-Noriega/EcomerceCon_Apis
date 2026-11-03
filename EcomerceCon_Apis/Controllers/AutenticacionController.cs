using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Login;
using EcomerceCon_Apis.Models;
using EcomerceCon_Apis.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcomerceCon_Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
  
         private readonly IUsuarioService _usuario;

        public AutenticacionController(IUsuarioService a)
        {
 ;
            _usuario = a;
        }
        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<LoginRespondoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RespuestaGenericaDto<LoginRespondoDto>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> login([FromBody] VerificarCuentaDto request)
        {

            var respuesta =  await _usuario.ValidarUsuario(request);

            return Ok(respuesta);

        }
    }
}
