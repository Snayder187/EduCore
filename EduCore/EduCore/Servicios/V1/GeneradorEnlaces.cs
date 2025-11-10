using EduCore.DTOs;
using Microsoft.AspNetCore.Authorization;
using System;

namespace EduCore.Servicios.V1
{
    public class GeneradorEnlaces : IGeneradorEnlaces
    {
        private readonly LinkGenerator linkGenerator;
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public GeneradorEnlaces(LinkGenerator linkGenerator,
            IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            this.linkGenerator = linkGenerator;
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ColeccionDeRecursosDTO<ApoderadoDTO>> 
            GenerarEnlaces(List<ApoderadoDTO> apoderados)
        {
            var resultado = new ColeccionDeRecursosDTO<ApoderadoDTO> { Valores = apoderados };

            var usuario = httpContextAccessor.HttpContext!.User;
            var esAdmin = await authorizationService.AuthorizeAsync(usuario, "esadmin");

            foreach (var dto in apoderados)
            {
                GenerarEnlaces(dto, esAdmin.Succeeded);
            }

            resultado.Enlaces.Add(new DatosHATEOASDTO(
                Enlace: linkGenerator.GetUriByRouteValues(httpContextAccessor.HttpContext!, 
                "ObtenerApoderadosV1", new { })!,
                Descripcion: "self",
                Metodo: "GET"
                ));

            if (esAdmin.Succeeded)
            {
                resultado.Enlaces.Add(new DatosHATEOASDTO(
                    Enlace: linkGenerator.GetUriByRouteValues(httpContextAccessor.HttpContext!, 
                    "CrearApoderadoV1", new { })!,
                    Descripcion: "apoderado-crear",
                    Metodo: "POST"
                    ));

                resultado.Enlaces.Add(new DatosHATEOASDTO(
                    Enlace: linkGenerator.GetUriByRouteValues(httpContextAccessor.HttpContext!, 
                    "CrearApoderadoConFotoV1", new { })!,
                    Descripcion: "apoderado-crear-con-foto",
                    Metodo: "POST"
                    ));
            }
            return resultado;
        }

        public async Task GenerarEnlaces(ApoderadoDTO apoderadoDTO)
        {
            var usuario = httpContextAccessor.HttpContext!.User;
            var esAdmin = await authorizationService.AuthorizeAsync(usuario, "esadmin");
            GenerarEnlaces(apoderadoDTO, esAdmin.Succeeded);

        }

        private void GenerarEnlaces(ApoderadoDTO apoderadoDTO, bool esAdmin)
        {
            apoderadoDTO.Enlaces.Add(
                    new DatosHATEOASDTO(
                    Enlace: linkGenerator.GetUriByRouteValues(httpContextAccessor.HttpContext!,
                    "ObtenerApoderadoV1", new { id = apoderadoDTO.Id })!,
                    Descripcion: "self",
                    Metodo: "GET"));

            if (esAdmin)
            {
                apoderadoDTO.Enlaces.Add(
                    new DatosHATEOASDTO(
                        Enlace: linkGenerator.GetUriByRouteValues(httpContextAccessor.HttpContext!,
                        "ActualizarApoderadoV1", new { id = apoderadoDTO.Id })!,
                    Descripcion: "apoderado-actualizar",
                    Metodo: "PUT"));

                apoderadoDTO.Enlaces.Add(
                    new DatosHATEOASDTO(
                        Enlace: linkGenerator.GetUriByRouteValues(httpContextAccessor.HttpContext!,
                        "PatchApoderadoV1", new { id = apoderadoDTO.Id })!,
                        Descripcion: "apoderado-patch",
                        Metodo: "PATCH"));

                apoderadoDTO.Enlaces.Add(
                    new DatosHATEOASDTO(
                        Enlace: linkGenerator.GetUriByRouteValues(httpContextAccessor.HttpContext!,
                        "BorrarApoderadoV1", new { id = apoderadoDTO.Id })!,
                        Descripcion: "apoderado-borrar",
                        Metodo: "DELETE"));
            }


        }
    }
}
