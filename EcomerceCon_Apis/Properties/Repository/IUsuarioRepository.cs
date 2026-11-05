using EcomerceCon_Apis.Models;
using Microsoft.AspNetCore.Identity;

namespace EcomerceCon_Apis.Properties.Repository
{
    public interface IUsuarioRepository
    {

        Task<Usuario> VerificarUsuario(string usuario);
        Task<bool> CheckearContra(Usuario usuario, string contraseña);
        Task<List<string>> ListaDeRolesDelUsuario(string userId);
        Task<List<string>> Roles(List<string> rolesid);
        Task<string> Idusuario(string email);
        Task<string> NombreRol(string id);
        Task CrearUsuario(Usuario usuario, string contraseña);
        Task agregarUserRoles(Usuario usuario);
        
    }
}
