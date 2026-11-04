using Azure.Core;
using EcomerceCon_Apis.Data;
using EcomerceCon_Apis.Models;
using MercadoPago.Resource.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcomerceCon_Apis.Properties.Repository
{
    public class UsuarioReposiory : IUsuarioRepository
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly ProductosDbcontext _context;
        public UsuarioReposiory(UserManager<Usuario> userManager, ProductosDbcontext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> CheckearContra(Usuario user, string contraseña)
        {
            return await _userManager.CheckPasswordAsync(user, contraseña);
        }

        public  async Task<List<string>> ListaDeRolesDelUsuario(string userId)
        {
           return await _context.UserRoles.Where(r => r.UserId == userId).Select(r => r.RoleId).ToListAsync();
        }

    

        public async Task<List<string>> Roles(List<string>rolesid)
        {
            return await _context.Roles.Where(r => rolesid.Contains(r.Id)).Select(r => r.Name).ToListAsync();
        }

        public async Task<Usuario> VerificarUsuario(string usuario)
        {
           return await _userManager.FindByEmailAsync(usuario);

        }

        public async Task<string> Idusuario(string email)
        {
           return await _context.Users.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefaultAsync();
        }

      public async Task<string>NombreRol(string id)
        {
            return await _context.Roles.Where(r => r.Id == id).Select(r => r.Name).FirstOrDefaultAsync();
        }

        public async Task CrearUsuario(Usuario usuario,string contraseña)
        {
            await _userManager.CreateAsync(usuario, contraseña);
        }

        public async Task agregarUserRoles(Usuario user)
        {
            _context.UserRoles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = "2" });
            await _context.SaveChangesAsync();
        }
    }
}

        

