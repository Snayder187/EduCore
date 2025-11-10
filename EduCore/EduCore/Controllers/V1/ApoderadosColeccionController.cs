using AutoMapper;
using EduCore.Datos;
using EduCore.DTOs;
using EduCore.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduCore.Controllers.V1
{
    [ApiController]
    [Route("api/v1/apoderados-coleccion")]
    [Authorize(Policy = "esadmin")]
    public class ApoderadosColeccionController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public ApoderadosColeccionController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{ids}", Name = "ObtenerApoderadosPorIdsV1")]
        public async Task<ActionResult<List<ApoderadoConAlumnoDTO>>> Get(string ids)
        {
            var idsColeccion = new List<int>();
            foreach (var id in ids.Split(","))
            {
                if (int.TryParse(id, out int idInt))
                {
                    idsColeccion.Add(idInt);
                }
            }

            if (!idsColeccion.Any())
            {
                ModelState.AddModelError(nameof(ids), "Ningún Id fue encontrado");
                return ValidationProblem();
            }

            var apoderados = await context.Apoderados
                                .Include(x => x.Alumnos)
                                    .ThenInclude(x => x.Alumno)
                                .Where(x => idsColeccion.Contains(x.Id))
                                .ToListAsync();

            if (apoderados.Count != idsColeccion.Count)
            {
                return NotFound();
            }

            var apoderadosDTO = mapper.Map<List<ApoderadoConAlumnoDTO>>(apoderados);
            return apoderadosDTO;
        }

        [HttpPost(Name = "CrearApoderadosV1")]
        public async Task<ActionResult> Post(IEnumerable<ApoderadoCreacionDTO> apoderadoCreacionDTO)
        {
            var apoderados = mapper.Map<IEnumerable<Apoderado>>(apoderadoCreacionDTO);
            context.AddRange(apoderados);
            await context.SaveChangesAsync();

            var apoderadosDTO = mapper.Map<IEnumerable<ApoderadoDTO>>(apoderados);
            var ids = apoderados.Select(x => x.Id);
            var idsString = string.Join(",", ids);
            return CreatedAtRoute("ObtenerApoderadosPorIdsV1", new { ids = idsString }, apoderadosDTO);
        }
    }
}
