using EduCore.Datos;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EduCore.Utilidades
{
    public class FiltroValidacionApoderado : IAsyncActionFilter
    {
        private readonly ApplicationDBContext dBContext;

        public FiltroValidacionApoderado(ApplicationDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await next();
        }
    }
}
