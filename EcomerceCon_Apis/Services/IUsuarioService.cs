using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Login;
using EcomerceCon_Apis.Dtos.Usuarios;

namespace EcomerceCon_Apis.Services
{
    public interface IUsuarioService
    {

        Task<RespuestaGenericaDto<LoginRespondoDto>> ValidarUsuario(VerificarCuentaDto user);
        Task<RespuestaGenericaDto<LoginRespondoDto>> CrearUsuario(CrearUsuarioDto user);
    }
}
