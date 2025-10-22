using AutoMapper;
using Azure;
using EduCore.Datos;
using EduCore.DTOs;
using EduCore.Entidades;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduCore.Controllers
{
    [ApiController]
    [Route("api/alumnos")]
    public class AlumnoController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        //Esto es Inyección de Dependencia
        public AlumnoController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<AlumnoDTO>> Get()
        {
            //logger.LogInformation("Obteniendo el listado de Alumnos");
            var alumnos = await context.Alumnos.ToListAsync();
            var alumnosDTO = mapper.Map<IEnumerable<AlumnoDTO>>(alumnos);
            return alumnosDTO;
        }

        [HttpGet("{id:int}", Name = "ObtenerAlumno")]
        public async Task<ActionResult<AlumnoConApoderadosDTO>> Get(int id)
        {
            var alumno = await context.Alumnos
                .Include(x => x.Apoderados)
                    .ThenInclude(x => x.Apoderado)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (alumno is null)
            {
                return NotFound();
            }

            var alumnoDTO = mapper.Map<AlumnoConApoderadosDTO>(alumno);
            return alumnoDTO;
        }

        [HttpPost]
        public async Task<ActionResult> Post(AlumnoCreacionDTO alumnoCreacionDTO)
        {
            if (alumnoCreacionDTO.ApoderadosIds is null || alumnoCreacionDTO.ApoderadosIds.Count == 0)
            {
                ModelState.AddModelError(nameof(alumnoCreacionDTO.ApoderadosIds), "No se puede crear un alumno sin apoderado");
                return ValidationProblem();
            }

            var apoderadoIdsExisten = await context.Apoderados
                                        .Where(x => alumnoCreacionDTO.ApoderadosIds.Contains(x.Id))
                                        .Select(x => x.Id).ToListAsync();

            if (apoderadoIdsExisten.Count != alumnoCreacionDTO.ApoderadosIds.Count)
            {
                var apoderadosNoExisten = alumnoCreacionDTO.ApoderadosIds.Except(apoderadoIdsExisten);
                var apoderadosNoExistenString = string.Join(",", apoderadosNoExisten);
                var mensajeError = $"Los siguientes autores no existen: {apoderadosNoExistenString}";
                ModelState.AddModelError(nameof(alumnoCreacionDTO.ApoderadosIds), mensajeError);
                return ValidationProblem();
            }

            var alumno = mapper.Map<Alumno>(alumnoCreacionDTO);
            AsignarOrdenApoderados(alumno);

            context.Add(alumno);
            await context.SaveChangesAsync();
            var alumnoDTO = mapper.Map<AlumnoDTO>(alumno);
            return CreatedAtRoute("ObtenerAlumno", new { id = alumno.Id }, alumnoDTO);
        }

        private void AsignarOrdenApoderados(Alumno alumno)
        {
            if (alumno.Apoderados is not null)
            {
                for (int i = 0; i < alumno.Apoderados.Count; i++)
                {
                    alumno.Apoderados[i].Orden = i;
                }
            }
        }

        [HttpPut("{id:int}")] // api/alumnos/id
        public async Task<ActionResult> Put(int id, AlumnoCreacionDTO alumnoCreacionDTO)
        {
            if (alumnoCreacionDTO.ApoderadosIds is null || alumnoCreacionDTO.ApoderadosIds.Count == 0)
            {
                ModelState.AddModelError(nameof(alumnoCreacionDTO.ApoderadosIds), "No se puede crear un alumno sin apoderado");
                return ValidationProblem();
            }

            var apoderadoIdsExisten = await context.Apoderados
                                        .Where(x => alumnoCreacionDTO.ApoderadosIds.Contains(x.Id))
                                        .Select(x => x.Id).ToListAsync();

            if (apoderadoIdsExisten.Count != alumnoCreacionDTO.ApoderadosIds.Count)
            {
                var apoderadosNoExisten = alumnoCreacionDTO.ApoderadosIds.Except(apoderadoIdsExisten);
                var apoderadosNoExistenString = string.Join(",", apoderadosNoExisten);
                var mensajeError = $"Los siguientes autores no existen: {apoderadosNoExistenString}";
                ModelState.AddModelError(nameof(alumnoCreacionDTO.ApoderadosIds), mensajeError);
                return ValidationProblem();
            }

            var alumnoDB = await context.Alumnos
                                .Include(x => x.Apoderados)
                                .FirstOrDefaultAsync(x => x.Id == id);

            if (alumnoDB is null)
            {
                return NotFound();
            }

            alumnoDB = mapper.Map(alumnoCreacionDTO, alumnoDB);
            AsignarOrdenApoderados(alumnoDB);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<AlumnoPatchDTO> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest();
            }
            var alumnoDB = await context.Alumnos.FirstOrDefaultAsync(x => x.Id == id);
            if (alumnoDB is null)
            {
                return NotFound();
            }
            var alumnoPatchDTO = mapper.Map<AlumnoPatchDTO>(alumnoDB);
            patchDoc.ApplyTo(alumnoPatchDTO, ModelState);
            var esValido = TryValidateModel(alumnoPatchDTO);
            if (!esValido)
            {
                return ValidationProblem();
            }
            mapper.Map(alumnoPatchDTO, alumnoDB);
            await context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var registroBorrado = await context.Alumnos.Where(x => x.Id == id).ExecuteDeleteAsync();
            if (registroBorrado == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
