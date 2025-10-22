using EduCore.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace EduCore.Entidades
{
    public class Alumno
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo {0} debe tener {1} carácteres o menos")]
        [PrimeraLetraMayuscula]
        public required string Nombres { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(80, ErrorMessage = "El campo {0} debe tener {1} carácteres o menos")]
        [PrimeraLetraMayuscula]
        public required string ApellidoPaterno { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(80, ErrorMessage = "El campo {0} debe tener {1} carácteres o menos")]
        [PrimeraLetraMayuscula]
        public required string ApellidoMaterno { get; set; }
        [Required]
        public List<ApoderadoAlumno> Apoderados { get; set; } = [];
        public List<Matricula> Matriculas { get; set; } = [];
    }
}
