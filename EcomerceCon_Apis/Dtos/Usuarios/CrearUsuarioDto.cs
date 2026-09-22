namespace EcomerceCon_Apis.Dtos.Usuarios
{
    public class CrearUsuarioDto
    {
        public string Correo { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime Fecha_nac { get; set; }
        public string Password { get; set; }


    }
}
