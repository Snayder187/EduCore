using EduCore.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace EduCore.DTOs
{
    public class ApoderadoPatchDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} carácteres o menos")]
        [PrimeraLetraMayuscula]
        public string Nombres { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} carácteres o menos")]
        [PrimeraLetraMayuscula]
        public string ApellidoPaterno { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} carácteres o menos")]
        [PrimeraLetraMayuscula]
        public string ApellidoMaterno { get; set; }
    }
}
