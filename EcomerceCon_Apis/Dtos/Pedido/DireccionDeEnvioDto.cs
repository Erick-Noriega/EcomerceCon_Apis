using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EcomerceCon_Apis.Dtos.Pedido
{
    public class DireccionDeEnvioDto
    {
        public string provincia { get; set; }
        public string Localidad { get; set; }=string.Empty;
        public string calle { get; set; }
        public string piso_depto { get; set; } = string.Empty;
        public string DireccionCompleta =>
        string.IsNullOrWhiteSpace(Localidad)
            ? $"{calle}, {Localidad}, {provincia}, Argentina"
            : $"{calle} ({Localidad}), {Localidad}, {provincia}, Argentina";

    }
    public class RespuestaPagoDto()
    {
        public int PedidoId { get; set; }
        public string InitPoint { get; set; } = string.Empty;
    }
}
