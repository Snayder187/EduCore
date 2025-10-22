using System.ComponentModel.DataAnnotations;

namespace EduCore.Entidades
{
    public class Comentario
    {
        public Guid Id { get; set; }
        [Required]
        public required string Cuerpo { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public bool Activo { get; set; }
        public string IpAddressRegistro { get; set; }
        public int UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public string? IpAddressModifica { get; set; }
        public int? UsuarioModifica { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}
