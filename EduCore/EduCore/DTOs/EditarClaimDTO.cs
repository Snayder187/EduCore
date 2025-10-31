using System.ComponentModel.DataAnnotations;

namespace EduCore.DTOs
{
    public class EditarClaimDTO
    {
        [EmailAddress]
        [Required]
        public required string Email { get; set; }
    }
}
