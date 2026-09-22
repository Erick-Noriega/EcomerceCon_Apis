namespace EcomerceCon_Apis.Dtos
{
    public class RespuestaGenericaDto<T>
    {
        public bool IsSuccess { get; set; }
        public string Mensaje { get; set; }
        public T? Data { get; set; }

    }
}
