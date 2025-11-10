using EduCore.Validaciones;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EduCore.Entidades
{
    public class Apoderado
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(120, ErrorMessage = "El campo {0} debe tener {1} carácteres o menos")]
        [PrimeraLetraMayuscula]
        public string Nombres { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(80, ErrorMessage = "El campo {0} debe tener {1} carácteres o menos")]
        [PrimeraLetraMayuscula]
        public string ApellidoPaterno { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(80, ErrorMessage = "El campo {0} debe tener {1} carácteres o menos")]
        [PrimeraLetraMayuscula]
        public string ApellidoMaterno { get; set; }
        [Unicode(false)]
        public string? Foto { get; set; }
        [Required]
        public List<ApoderadoAlumno> Alumnos { get; set; } = [];
    }
}
