using EduCore.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduCore.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRootV1")]
        [AllowAnonymous]
        public async Task<IEnumerable<DatosHATEOASDTO>> Get()
        {
            var datosHATEOAS = new List<DatosHATEOASDTO>();

            var esAdmin = await authorizationService.AuthorizeAsync(User, "esadmin");

            //Acciones que cualquiera puede realizar
            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerRootV1", new { })!,
                Descripcion: "self", Metodo: "GET"));

            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerApoderadosV1", new { })!,
                Descripcion: "apoderados-obtener", Metodo: "GET"));

            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("RegistroUsuarioV1", new { })!,
                Descripcion: "usuario-registro", Metodo: "GET"));

            datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("LoginUsuarioV1", new { })!,
                Descripcion: "usuario-login", Metodo: "GET"));

            //Acciones para usuarios logueados
            if (User.Identity!.IsAuthenticated)
            {
                datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ActualizarUsuarioV1", new { })!,
                Descripcion: "usuario-actualizar", Metodo: "GET"));

                datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("RenovarTokenV1", new { })!,
                    Descripcion: "token-renovar", Metodo: "GET"));
            }

            //Acciones que solo usuarios admins pueden realizar
            if (esAdmin.Succeeded)
            {
                datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("CrearApoderadoV1", new { })!,
                Descripcion: "apoderado-crear", Metodo: "GET"));

                datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("CrearApoderadosV1", new { })!,
                    Descripcion: "apoderados-crear", Metodo: "GET"));

                datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("CrearAlumnoV1", new { })!,
                    Descripcion: "alumno-crear", Metodo: "GET"));

                datosHATEOAS.Add(new DatosHATEOASDTO(Enlace: Url.Link("ObtenerAlumnosV1", new { })!,
                    Descripcion: "alumnos-obtener", Metodo: "GET"));
            }
            return datosHATEOAS;
        }
    }
}
