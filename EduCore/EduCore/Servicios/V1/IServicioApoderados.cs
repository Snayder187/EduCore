using EduCore.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EduCore.Servicios.V1
{
    public interface IServicioApoderados
    {
        Task<IEnumerable<ApoderadoDTO>> Get(PaginacionDTO paginacionDTO);
    }
}