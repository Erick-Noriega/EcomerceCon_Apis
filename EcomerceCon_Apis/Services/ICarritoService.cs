namespace EcomerceCon_Apis.Services
{
    public interface ICarritoService
    {
        Task VaciarCarritoDelUsuario(string usuarioId);
    }
}
