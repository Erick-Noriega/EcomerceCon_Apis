using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Login;
using EcomerceCon_Apis.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly string _secretkey;
        private readonly  UserManager<Usuario>_userManager;
        public AutenticacionController(IOptions<JwtSettings> conf, UserManager<Usuario> user)
        {
            _secretkey = conf.Value.SecretKey;
            _userManager = user;
        }
        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<LoginRespondoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RespuestaGenericaDto<LoginRespondoDto>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> login([FromBody]VerificarCuentaDto request)
        {
            bool aux = false;
            var user = _userManager.FindByEmailAsync(request.correo);
            if (user != null)
            {
                aux = true;
            }
            bool EsCorrecta=await _userManager.CheckPasswordAsync(await user, request.clave);
            if (EsCorrecta == true && aux == true)
            {
                var keybytes = Encoding.UTF8.GetBytes(_secretkey);
                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.correo));
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
                        Correo = request.correo
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



    }
}
