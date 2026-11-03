using EcomerceCon_Apis.Dtos;
using EcomerceCon_Apis.Dtos.Login;

namespace EcomerceCon_Apis.Services
{
    public interface IUsuarioService
    {

        Task<RespuestaGenericaDto<LoginRespondoDto>> ValidarUsuario(VerificarCuentaDto user);
    }
}
