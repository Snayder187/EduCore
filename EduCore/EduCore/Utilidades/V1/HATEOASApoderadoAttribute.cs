using EduCore.DTOs;
using EduCore.Servicios.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EduCore.Utilidades.V1
{
    public class HATEOASApoderadoAttribute: HATEOASFilterAtribute
    {
        private readonly IGeneradorEnlaces generadorEnlaces;

        public HATEOASApoderadoAttribute(IGeneradorEnlaces generadorEnlaces)
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
            var modelo = result!.Value as ApoderadoDTO ?? 
                throw new ArgumentNullException("Se esperaba una instancia de ApoderadoDTO");
            await generadorEnlaces.GenerarEnlaces(modelo);
            await next();
        }
    }
}
