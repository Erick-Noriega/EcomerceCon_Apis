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

        public async Task<string>ObtenerNumeroUsaurio(string UsuarioId)
        {
            return await _context.Users.Where(u => u.Id == UsuarioId).Select(u => u.Numero_telefono).FirstOrDefaultAsync();
        }

        public async Task<List<string>> ObtenerClavesRandom()
        {
            return await _context.Users
                .Select(usuario => usuario.ClaveRandomaaa)
                .ToListAsync();
        }

        public async Task<Usuario?> ObtenerUsuarioPorId(string usuarioId)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(usuario => usuario.Id == usuarioId);
        }

        public async Task<Usuario?> ObtenerUsuarioPorClaveAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
            {
                return null;
            }

            var claveNormalizada = clave.Trim().ToUpperInvariant();
            return await _context.Users
                .AsNoTracking()
                .Where(usuario => usuario.ClaveRandomaaa.ToUpper() == claveNormalizada)
                .Select(usuario => new Usuario
                {
                    Id = usuario.Id,
                    UserName = usuario.UserName,
                    Email = usuario.Email,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Fecha_nac = usuario.Fecha_nac,
                    Numero_telefono = usuario.Numero_telefono,
                    ClaveRandomaaa = usuario.ClaveRandomaaa
                })
                .FirstOrDefaultAsync();
        }
    }
}

        

