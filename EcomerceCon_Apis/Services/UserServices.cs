using Azure.Core;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Login;
using EcomerceCon_Apis.Models;
using EcomerceCon_Apis.Properties.Repository;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcomerceCon_Apis.Services
{
    public class UserServices : IUsuarioService

    {
        private readonly string _secretkey;
        private readonly IUsuarioRepository _usuarioRepository;
        public UserServices(IUsuarioRepository usuarioRepository, IOptions<JwtSettings> conf)
        {
            _usuarioRepository = usuarioRepository;
            _secretkey = conf.Value.SecretKey;
        }
        public async Task<RespuestaGenericaDto<LoginRespondoDto>> ValidarUsuario(VerificarCuentaDto usuario)
        {
            bool aux = false;
            var user = await _usuarioRepository.VerificarUsuario(usuario.correo);
            if (user != null)
            {
                aux = true;
            }
            if (user != null && await _usuarioRepository.CheckearContra(user, usuario.clave))
            {
                var keybytes = Encoding.UTF8.GetBytes(_secretkey);

                var claims = new ClaimsIdentity();

                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                claims.AddClaim(new Claim(ClaimTypes.Email, usuario.correo));
                var userId = await _usuarioRepository.Idusuario(usuario.correo);
                nulos(userId);
                var rolesid = await _usuarioRepository.ListaDeRolesDelUsuario(userId);
                if (rolesid == null)
                {
                    return new RespuestaGenericaDto<LoginRespondoDto>
                    {
                        IsSuccess = false,
                        Mensaje = "rol no encontrado",
                    };
                }


                var roles = await _usuarioRepository.Roles(rolesid);
                if (rolesid == null)
                {
                    return new RespuestaGenericaDto<LoginRespondoDto>
                    {
                        IsSuccess = false,
                        Mensaje = "rol no encontrado",
                    };
                }
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
                nulos(roleid);
                var role = await _usuarioRepository.NombreRol(roleid);
                nulos(role);

                var respuesta = new RespuestaGenericaDto<LoginRespondoDto>
                {
                    IsSuccess = true,
                    Mensaje = "Usuario verificado correctamente",
                    Data = new LoginRespondoDto
                    {
                        Token = tokencreado,
                        Correo = usuario.correo,
                        Role = role
                    }
                };

                return respuesta;
            }
            else
            {
                var respuestaError = new RespuestaGenericaDto<LoginRespondoDto>
                {
                    IsSuccess = false,
                    Mensaje = "Usuario o contraseña incorrectos",
                    Data = null
                };
                return respuestaError;
            }



            }

        private RespuestaGenericaDto<LoginRespondoDto>  nulos(string id)
        {
            if (id == null)
            {
               return new RespuestaGenericaDto<LoginRespondoDto>
                {
                    IsSuccess = false,
                    Mensaje = "rol no encontrado",
                };
            }
            return new RespuestaGenericaDto<LoginRespondoDto>
            {
                IsSuccess = true,
                Mensaje = "rol no encontrado",
            };
        }

        }
    }

