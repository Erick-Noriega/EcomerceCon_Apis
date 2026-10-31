using Microsoft.AspNetCore.Identity;

namespace EcomerceCon_Apis.Models
 
{
    
    public class Usuario:IdentityUser
    { 
        public string Nombre { get; set; }
        public string Apellido { get; set; } 
        public DateTime Fecha_nac { get; set; }
        public string? Numero_telefono { get; set; }
        List<Pedido>? pedidos { get; set; }

    }
}
