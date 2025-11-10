using EduCore.DTOs;

namespace EduCore.Servicios.V1
{
    public interface IGeneradorEnlaces
    {
        Task GenerarEnlaces(ApoderadoDTO apoderadoDTO);
        Task<ColeccionDeRecursosDTO<ApoderadoDTO>> GenerarEnlaces(List<ApoderadoDTO> apoderados);
    }
}