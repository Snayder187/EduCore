using AutoMapper;
using Azure;
using EduCore.Datos;
using EduCore.DTOs;
using EduCore.Entidades;
using EduCore.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace EduCore.Controllers.V2
{
    [ApiController]
    [Route("api/v2/alumnos")]
    [Authorize(Policy = "esadmin")]
    public class AlumnoController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;
        private readonly ITimeLimitedDataProtector protectorLimitadoPorTiempo;
        private const string cache = "alumnos-obtener";

        //Esto es Inyección de Dependencia
        public AlumnoController(ApplicationDBContext context, IMapper mapper,
            IOutputCacheStore outputCacheStore)
        {
            this.context = context;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
        }

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Tags = [cache])]
        public async Task<IEnumerable<AlumnoDTO>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Alumnos.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var alumnos = await queryable
                        .OrderBy(x => x.Nombres)
                        .Paginar(paginacionDTO).ToListAsync();
            var alumnosDTO = mapper.Map<IEnumerable<AlumnoDTO>>(alumnos);
            return alumnosDTO;
        }

        [HttpGet("{id:int}", Name = "ObtenerAlumnoV2")]
        [AllowAnonymous]
        [OutputCache(Tags = [cache])]
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
        [ServiceFilter<FiltroValidacionAlumno>()]
        public async Task<ActionResult> Post(AlumnoCreacionDTO alumnoCreacionDTO)
        {
            var alumno = mapper.Map<Alumno>(alumnoCreacionDTO);
            AsignarOrdenApoderados(alumno);

            context.Add(alumno);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);
            var alumnoDTO = mapper.Map<AlumnoDTO>(alumno);
            return CreatedAtRoute("ObtenerAlumnoV2", new { id = alumno.Id }, alumnoDTO);
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
        [ServiceFilter<FiltroValidacionAlumno>()]
        public async Task<ActionResult> Put(int id, AlumnoCreacionDTO alumnoCreacionDTO)
        {
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
            await outputCacheStore.EvictByTagAsync(cache, default);
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
            await outputCacheStore.EvictByTagAsync(cache, default);
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
            await outputCacheStore.EvictByTagAsync(cache, default);
            return NoContent();
        }
    }
}
