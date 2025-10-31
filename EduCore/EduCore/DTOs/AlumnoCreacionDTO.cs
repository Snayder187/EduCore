using EduCore.Entidades;
using EduCore.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace EduCore.DTOs
{
    public class AlumnoCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} carácteres o menos")]
        [PrimeraLetraMayuscula]
        public required string Nombres { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(80, ErrorMessage = "El campo {0} debe tener {1} carácteres o menos")]
        public required string ApellidoPaterno { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(80, ErrorMessage = "El campo {0} debe tener {1} carácteres o menos")]
        public required string ApellidoMaterno { get; set; }
        public List<int> ApoderadosIds { get; set; } = [];
    }
}
