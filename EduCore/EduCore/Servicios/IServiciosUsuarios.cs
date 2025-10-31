using EduCore.Entidades;
using Microsoft.AspNetCore.Identity;

namespace EduCore.Servicios
{
    public interface IServiciosUsuarios
    {
        Task<Usuario?> ObtenerUsuario();
    }
}