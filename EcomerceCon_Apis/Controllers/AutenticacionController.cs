using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Login;
using EcomerceCon_Apis.Models;
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
        private readonly string _secretkey;
        private readonly UserManager<Usuario> _userManager;
        private readonly ProductosDbcontext _context;
        public AutenticacionController(IOptions<JwtSettings> conf, UserManager<Usuario> user, ProductosDbcontext context)
        {
            _secretkey = conf.Value.SecretKey;
            _userManager = user;
            _context = context;
        }
        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(typeof(RespuestaGenericaDto<LoginRespondoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RespuestaGenericaDto<LoginRespondoDto>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> login([FromBody] VerificarCuentaDto request)
        {

            bool aux = false;
            var user = await _userManager.FindByEmailAsync(request.correo);
            var userId=await _context.Users.Where(u => u.Email == request.correo).Select(u => u.Id).FirstOrDefaultAsync();

            if (user != null)
            {
                aux = true;
            }

            if (user != null &&
                await _userManager.CheckPasswordAsync(user, request.clave))
            {
                var keybytes = Encoding.UTF8.GetBytes(_secretkey);

                var claims = new ClaimsIdentity();

                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                claims.AddClaim(new Claim(ClaimTypes.Email, request.correo));

                // Obtener los roles reales del usuario
                var rolesid = await _context.UserRoles.Where(r=>r.UserId==userId).Select(r=>r.RoleId).ToListAsync();
                var roles = await _context.Roles.Where(r => rolesid.Contains(r.Id)).Select(r => r.Name).ToListAsync();

                foreach (var rol in roles)
                {
                    claims.AddClaim(new Claim(ClaimTypes.Role, rol));
                }

                var tokendescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(keybytes),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokendescriptor);
                var tokencreado = tokenHandler.WriteToken(tokenConfig);

                var roleid = rolesid.FirstOrDefault();
                var role = await _context.Roles.Where(r => r.Id == roleid).Select(r => r.Name).FirstOrDefaultAsync();

                var respuesta = new RespuestaGenericaDto<LoginRespondoDto>
                {
                    IsSuccess = true,
                    Mensaje = "Usuario verificado correctamente",
                    Data = new LoginRespondoDto
                    {
                        Token = tokencreado,
                        Correo = request.correo,
                        Role = role
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
