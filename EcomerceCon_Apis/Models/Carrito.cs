using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace EcomerceCon_Apis.Models
{
    public class Carrito
    {
        public int Id { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        public Usuario? usuario { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public List<DetalleCarrito>? DetalleCarrito { get; set; }


    }
}
