using EduCore.DTOs;
using EduCore.Servicios.V1;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace EduCore.Utilidades.V1
{
    public class HATEOASApoderadosAttribute : HATEOASFilterAtribute
    {
        private readonly IGeneradorEnlaces generadorEnlaces;

        public HATEOASApoderadosAttribute(IGeneradorEnlaces generadorEnlaces)
        {
            this.generadorEnlaces = generadorEnlaces;
        }

        public override async Task OnResultExecutionAsync
            (ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var incluirHATEOAS = DebeIncluirHATEOAS(context);
            if (!incluirHATEOAS)
            {
                await next();
                return;
            }

            var result = context.Result as ObjectResult;
            var modelo = result!.Value as List<ApoderadoDTO> ??
                throw new ArgumentNullException("Se esperaba una instancia de List<ApoderadoDTO>");
            context.Result = new OkObjectResult(await generadorEnlaces.GenerarEnlaces(modelo));
            await next();
        }
    }
}
