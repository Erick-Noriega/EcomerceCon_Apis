namespace ConsumirapideEcomerceee.Models
{
    public class DireccionDeEnviosDto
    {
        public string provincia { get; set; }
        public string Localidad { get; set; }
        public string calle { get; set; }
        public string piso_depto { get; set; } = string.Empty;
        public string DireccionCompleta =>
        string.IsNullOrWhiteSpace(Localidad)
            ? $"{calle}, {Localidad}, {provincia}, Argentina"
            : $"{calle} ({Localidad}), {Localidad}, {provincia}, Argentina";
    }
}
