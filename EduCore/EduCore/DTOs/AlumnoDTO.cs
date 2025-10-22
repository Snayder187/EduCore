using EduCore.Entidades;
using EduCore.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace EduCore.DTOs
{
    public class AlumnoDTO
    {
        public int Id { get; set; }
        public required string Nombres { get; set; }
        public required string Apellidos { get; set; }
    }
}
