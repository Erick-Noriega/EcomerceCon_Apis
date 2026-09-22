namespace EcomerceCon_Apis.Dtos.Pagos
{
    public class MercadoPagoWebhookDto
    {
        public string? Action { get; set; }
        public string? Type { get; set; }
        public MercadoPagoDataDto? Data { get; set; }
    }

    public class MercadoPagoDataDto
    {
        public string? Id { get; set; }
    }
}
