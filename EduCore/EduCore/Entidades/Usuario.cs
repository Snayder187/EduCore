using Microsoft.AspNetCore.Identity;

namespace EduCore.Entidades
{
    public class Usuario: IdentityUser
    {
        public DateTime FechaNacimiento { get; set; }
    }
}
