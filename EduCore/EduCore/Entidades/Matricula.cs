using EduCore.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace EduCore.Entidades
{
    public class Matricula
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100, ErrorMessage = "El campo {0} debe tener {1} carácteres o menos")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        public int AlumnoId { get; set; }
        public Alumno? Alumno { get; set; }
        public bool Activo { get; set; }
        public string IpAddressRegistro { get; set; }
        public int UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public string? IpAddressModifica { get; set; }
        public int? UsuarioModifica { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}
