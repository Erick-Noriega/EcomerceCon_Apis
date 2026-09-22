namespace EcomerceCon_Apis.Dtos.Pagos
{
    public class CrearPagoDto
    {
        public int PedidoId { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string TransaccionId { get; set; } = string.Empty; // ID devuelto por la pasarela
        //public float  monto { get; set; }
    }

}
