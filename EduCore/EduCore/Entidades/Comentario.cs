using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EduCore.Entidades
{
    public class Comentario
    {
        public Guid Id { get; set; }
        [Required]
        public required string Cuerpo { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public int ApoderadoId { get; set; }
        public Apoderado? Apoderado { get; set; }
        public required string UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
