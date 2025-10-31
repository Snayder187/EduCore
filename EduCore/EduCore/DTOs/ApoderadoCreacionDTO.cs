using EduCore.Entidades;
using System.ComponentModel.DataAnnotations;

namespace EduCore.DTOs
{
    public class ApoderadoCreacionDTO
    {
        [Required]
        public string Nombres { get; set; }
        [Required]
        public string ApellidoPaterno { get; set; }
        [Required]
        public string ApellidoMaterno { get; set; }
        public List<AlumnoCreacionDTO> Alumnos { get; set; } = [];
    }
}
