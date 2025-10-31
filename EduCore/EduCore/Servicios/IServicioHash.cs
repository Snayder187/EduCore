using EduCore.DTOs;

namespace EduCore.Servicios
{
    public interface IServicioHash
    {
        ResultadoHashDTO Hash(string input);
        ResultadoHashDTO Hash(string input, byte[] sal);
    }
}