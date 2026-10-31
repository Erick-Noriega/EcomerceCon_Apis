namespace ConsumirapideEcomerceee.Models
{
    public class ResultadoApi<T>
    {
        public bool IsSuccess { get; set; }
        public string Mensake { get; set; }
       
        public T Data { get; set; }


    }
}
