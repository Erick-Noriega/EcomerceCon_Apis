using Azure.Core;
using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Login;
using EcomerceCon_Apis.Dtos.Usuarios;
using EcomerceCon_Apis.Models;
using EcomerceCon_Apis.Properties.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EcomerceCon_Apis.Services
{
    public class UserServices : IUsuarioService

    {
        private readonly string _secretkey;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICarritoRepository _carritoRepository;
        private readonly SignInManager<Usuario> _signInManager;
        public UserServices(IUsuarioRepository usuarioRepository, IOptions<JwtSettings> conf, ICarritoRepository carritoRepository, SignInManager<Usuario> signInManager)
        {
            _usuarioRepository = usuarioRepository;
            _secretkey = conf.Value.SecretKey;
            _carritoRepository = carritoRepository;
            _signInManager = signInManager;
        }

        public async Task<RespuestaGenericaDto<LoginRespondoDto>> CrearUsuario(CrearUsuarioDto user)
        {
            if (await _usuarioRepository.VerificarUsuario(user.Correo) == null)
            {
                user.ClaveRandomaaa = await GenerarClaveRandomUnica();
                var usuario = new Usuario
                {
                    UserName = user.Correo,
                    Email = user.Correo,
                    Nombre = user.Nombre,
                    Apellido = user.Apellido,
                    Fecha_nac = user.Fecha_nac,
                    ClaveRandomaaa = user.ClaveRandomaaa
                };

                await _usuarioRepository.CrearUsuario(usuario, user.Password);
                var nuevocarrito = new Carrito
                {
                    UsuarioId = usuario.Id,
                    Fecha = DateTime.Now
                };
                await _carritoRepository.CrearCarrito(nuevocarrito);
                await _usuarioRepository.agregarUserRoles(usuario);
                return new RespuestaGenericaDto<LoginRespondoDto>
                {
                    IsSuccess = true,
                    Mensaje = "Usuario registrado correctamente y se creo el carrito",
                };

            }
            else
            {
                return new RespuestaGenericaDto<LoginRespondoDto>
                {
                    IsSuccess = false,
                    Mensaje = "El correo ya está registrado",
                };
            }

        }

        private async Task<string> GenerarClaveRandomUnica()
        {
            var clavesExistentes = await _usuarioRepository.ObtenerClavesRandom();
            var claves = new HashSet<string>(clavesExistentes, StringComparer.OrdinalIgnoreCase);
            string clave;

            do
            {
                clave = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            }
            while (claves.Contains(clave));

            return clave;
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

        public async Task<RespuestaGenericaDto<LoginRespondoDto>> IniciarSesionConClaveRandom(string claveRandom)
        {
            var usuario = await _usuarioRepository.ObtenerUsuarioPorClaveAsync(claveRandom);
            if (usuario == null || string.IsNullOrWhiteSpace(usuario.Email))
            {
                return new RespuestaGenericaDto<LoginRespondoDto>
                {
                    IsSuccess = false,
                    Mensaje = "La clave o documento no fue reconocido."
                };
            }

            var rolesId = await _usuarioRepository.ListaDeRolesDelUsuario(usuario.Id);
            var roles = rolesId.Count == 0
                ? new List<string>()
                : await _usuarioRepository.Roles(rolesId);

            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id));
            claims.AddClaim(new Claim(ClaimTypes.Email, usuario.Email));

            foreach (var rol in roles.Where(rol => !string.IsNullOrWhiteSpace(rol)))
            {
                claims.AddClaim(new Claim(ClaimTypes.Role, rol));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretkey)),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));

            return new RespuestaGenericaDto<LoginRespondoDto>
            {
                IsSuccess = true,
                Mensaje = "Inicio de sesión por WhatsApp correcto.",
                Data = new LoginRespondoDto
                {
                    Token = token,
                    Correo = usuario.Email,
                    Role = roles.FirstOrDefault() ?? string.Empty
                }
            };
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

       public async Task<string> ObtenerIdUsuario(string email)
        {
            var idusario = await _usuarioRepository.Idusuario(email);
            if (idusario == null)
            {
                throw new Exception("No se encontró el usuario con el correo proporcionado.");
            }
            return idusario;
        }

        public async Task<UsuarioDatosDto?> ObtenerDatosUsuario(string usuarioId)
        {
            var usuario = await _usuarioRepository.ObtenerUsuarioPorId(usuarioId);
            if (usuario == null)
            {
                return null;
            }

            return new UsuarioDatosDto
            {
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo = usuario.Email ?? string.Empty,
                NumeroTelefono = usuario.Numero_telefono,
                ClaveRandomaaa = usuario.ClaveRandomaaa
            };
        }

        public Task CerrarSesion()
        {
            return _signInManager.SignOutAsync();
        }
    }
    }

