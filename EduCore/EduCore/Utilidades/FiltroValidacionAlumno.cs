using EduCore.Datos;
using EduCore.DTOs;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace EduCore.Utilidades
{
    public class FiltroValidacionAlumno : IAsyncActionFilter
    {
        private readonly ApplicationDBContext dbContext;

        public FiltroValidacionAlumno(ApplicationDBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue("alumnoCreacionDTO", out var value) ||
                value is not AlumnoCreacionDTO alumnoCreacionDTO)
            {
                context.ModelState.AddModelError(string.Empty, "El módelo enviado no es válido");
                context.Result = context.ModelState.ConstruirProblemDetail();
                return;
            }

            if (alumnoCreacionDTO.ApoderadosIds is null || alumnoCreacionDTO.ApoderadosIds.Count == 0)
            {
                context.ModelState.AddModelError(nameof(alumnoCreacionDTO.ApoderadosIds), "No se puede crear un alumno sin apoderado");
                context.Result = context.ModelState.ConstruirProblemDetail();
                return;
            }

            var apoderadoIdsExisten = await dbContext.Apoderados
                                        .Where(x => alumnoCreacionDTO.ApoderadosIds.Contains(x.Id))
                                        .Select(x => x.Id).ToListAsync();

            if (apoderadoIdsExisten.Count != alumnoCreacionDTO.ApoderadosIds.Count)
            {
                var apoderadosNoExisten = alumnoCreacionDTO.ApoderadosIds.Except(apoderadoIdsExisten);
                var apoderadosNoExistenString = string.Join(",", apoderadosNoExisten);
                var mensajeError = $"Los siguientes autores no existen: {apoderadosNoExistenString}";
                context.ModelState.AddModelError(nameof(alumnoCreacionDTO.ApoderadosIds), mensajeError);
                context.Result = context.ModelState.ConstruirProblemDetail();
                return;
            }

            await next();
        }
    }
}
