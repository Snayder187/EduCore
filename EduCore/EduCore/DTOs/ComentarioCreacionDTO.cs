using System.ComponentModel.DataAnnotations;

namespace EduCore.DTOs
{
    public class ComentarioCreacionDTO
    {
        [Required]
        public required string Cuerpo { get; set; }
    }
}
