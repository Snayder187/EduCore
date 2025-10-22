using AutoMapper;
using EduCore.Datos;
using EduCore.DTOs;
using EduCore.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduCore.Controllers
{
    [ApiController]
    [Route("api/apoderado")]
    public class ApoderadoController: ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public ApoderadoController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<ApoderadoDTO>> Get()
        {
            var apoderados = await context.Apoderados.ToListAsync();
            var apoderadosDTO = mapper.Map<IEnumerable<ApoderadoDTO>>(apoderados);
            return apoderadosDTO;
        }

        [HttpGet("{id:int}", Name = "ObtenerApoderado")]
        public async Task<ActionResult<ApoderadoConAlumnoDTO>> Get(int id)
        {
            var apoderado = await context.Apoderados
                .Include(x => x.Alumnos)
                    .ThenInclude(x => x.Alumno)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (apoderado is null)
            {
                return NotFound();
            }

            var apoderadoDTO = mapper.Map<ApoderadoConAlumnoDTO>(apoderado);
            return apoderadoDTO;
        }

        [HttpPost]
        public async Task<ActionResult> Post(ApoderadoCreacionDTO apoderadoCreacionDTO)
        {
            var apoderado = mapper.Map<Apoderado>(apoderadoCreacionDTO);
            context.Add(apoderado);
            await context.SaveChangesAsync();
            var apoderadoDTO = mapper.Map<ApoderadoDTO>(apoderado);
            return CreatedAtRoute("obtenercurso", new { id = apoderado.Id }, apoderadoDTO);
        }

        //[HttpPut("{id:int}")] // api/alumnos/id
        //public async Task<ActionResult> Put(int id, ApoderadoCreacionDTO apoderadoCreacionDTO)
        //{
        //    var apoderado = mapper.Map<Apoderado>(apoderadoCreacionDTO);
        //    apoderado.Id = id;
        //    context.Update(apoderado);
        //    await context.SaveChangesAsync();
        //    return NoContent();
        //}

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var registroBorrados = await context.Apoderados.Where(x => x.Id == id).ExecuteDeleteAsync();
            if (registroBorrados == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
