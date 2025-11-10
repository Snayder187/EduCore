using AutoMapper;
using Azure;
using EduCore.Datos;
using EduCore.DTOs;
using EduCore.Entidades;
using EduCore.Servicios;
using EduCore.Servicios.V1;
using EduCore.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq.Dynamic.Core;

namespace EduCore.Controllers.V2
{
    [ApiController]
    [Route("api/v2/apoderado")]
    [Authorize(Policy = "esadmin")]
    public class ApoderadoController: ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly ILogger<ApoderadoController> logger;
        private readonly IOutputCacheStore outputCacheStore;
        private readonly IServicioApoderados servicioApoderadosV1;
        private const string contenedor = "apoderados";
        private const string cache = "apoderados-obtener";

        public ApoderadoController(ApplicationDBContext context, IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos, ILogger<ApoderadoController> logger,
            IOutputCacheStore outputCacheStore, IServicioApoderados servicioApoderadosV1)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
            this.logger = logger;
            this.outputCacheStore = outputCacheStore;
            this.servicioApoderadosV1 = servicioApoderadosV1;
        }

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Tags = [cache])]
        public async Task<IEnumerable<ApoderadoDTO>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            return await servicioApoderadosV1.Get(paginacionDTO);
        }

        [HttpGet("{id:int}", Name = "ObtenerApoderadoV2")]
        [AllowAnonymous]
        [EndpointSummary("Obtiene apoderado por Id")]
        [EndpointDescription("Obtiene un apoderado por su Id. Incluye sus alumnos. Si el apoderado no existe, se retorna 404")]
        [ProducesResponseType<ApoderadoConAlumnoDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[OutputCache(Tags = [cache])]
        public async Task<ActionResult<ApoderadoConAlumnoDTO>> Get(
            [Description("El id del apoderado")]int id, bool incluirAlumnos = false)
        {
            var queryable = context.Apoderados.AsQueryable();
            if (incluirAlumnos)
            {
                queryable = queryable.Include(x => x.Alumnos)
                    .ThenInclude(x => x.Alumno);
            }
            var apoderado = await queryable.FirstOrDefaultAsync(x => x.Id == id);

            if (apoderado is null)
            {
                return NotFound();
            }

            var apoderadoDTO = mapper.Map<ApoderadoConAlumnoDTO>(apoderado);
            return apoderadoDTO;
        }

        [HttpGet("filtrar")]
        [AllowAnonymous]
        public async Task<ActionResult> Filtrar([FromQuery] ApoderadoFiltroDTO apoderadoFiltroDTO)
        {
            var queryable = context.Apoderados.AsQueryable();

            if (!string.IsNullOrEmpty(apoderadoFiltroDTO.Nombres))
            {
                queryable = queryable.Where(x => x.Nombres.Contains(apoderadoFiltroDTO.Nombres));
            }

            if (!string.IsNullOrEmpty(apoderadoFiltroDTO.ApellidoPaterno))
            {
                queryable = queryable.Where(x => x.ApellidoPaterno.Contains(apoderadoFiltroDTO.ApellidoPaterno));
            }

            if (apoderadoFiltroDTO.IncluirAlumnos)
            {
                queryable = queryable.Include(x => x.Alumnos).ThenInclude(x => x.Alumno);
            }

            if (apoderadoFiltroDTO.TieneFoto.HasValue)
            {
                if (apoderadoFiltroDTO.TieneFoto.Value)
                {
                    queryable = queryable.Where(x => x.Foto != null);
                }
                else
                {
                    queryable = queryable.Where(x => x.Foto == null);
                }
            }

            if (apoderadoFiltroDTO.TieneAlumnos.HasValue)
            {
                if (apoderadoFiltroDTO.TieneAlumnos.Value)
                {
                    queryable = queryable.Where(x => x.Alumnos.Any());
                }
                else
                {
                    queryable = queryable.Where(x => !x.Alumnos.Any());
                }
            }

            if (!string.IsNullOrEmpty(apoderadoFiltroDTO.NombreAlumno))
            {
                queryable = queryable.Where(x => 
                    x.Alumnos.Any(y => y.Alumno!.Nombres.Contains(apoderadoFiltroDTO.NombreAlumno)));
            }

            if (!string.IsNullOrEmpty(apoderadoFiltroDTO.CampoOrdenar))
            {
                var tipoOrden = apoderadoFiltroDTO.OrdenAscendente ? "ascending" : "descending";
                try
                {
                    queryable = queryable.OrderBy($"{apoderadoFiltroDTO.CampoOrdenar} {tipoOrden}");
                }
                catch (Exception ex)
                {
                    queryable = queryable.OrderBy(x => x.Nombres);
                    logger.LogError(ex.Message, ex);
                }
            }
            else
            {
                queryable = queryable.OrderBy(x => x.Nombres);
            }

            var apoderados = await queryable
                        .Paginar(apoderadoFiltroDTO.PaginacionDTO).ToListAsync();

            if (apoderadoFiltroDTO.IncluirAlumnos)
            {
                var apoderadosDTO = mapper.Map<IEnumerable<ApoderadoConAlumnoDTO>>(apoderados);
                return Ok(apoderadosDTO);
            }
            else
            {
                var apoderadosDTO = mapper.Map<IEnumerable<ApoderadoDTO>>(apoderados);
                return Ok(apoderadosDTO);
            }   
        }

        [HttpPost]
        public async Task<ActionResult> Post(ApoderadoCreacionDTO apoderadoCreacionDTO)
        {
            var apoderado = mapper.Map<Apoderado>(apoderadoCreacionDTO);
            context.Add(apoderado);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);
            var apoderadoDTO = mapper.Map<ApoderadoDTO>(apoderado);
            return CreatedAtRoute("ObtenerApoderadoV2", new { id = apoderado.Id }, apoderadoDTO);
        }

        [HttpPost("con-foto")]
        public async Task<ActionResult> PostConFoto([FromForm]ApoderadoCreacionDTOConFoto apoderadoCreacionDTO)
        {
            var apoderado = mapper.Map<Apoderado>(apoderadoCreacionDTO);

            if (apoderadoCreacionDTO.Foto is not null)
            {
                var url = await almacenadorArchivos.Almacenar(contenedor,
                    apoderadoCreacionDTO.Foto);
                apoderado.Foto = url;
            }

            context.Add(apoderado);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);
            var apoderadoDTO = mapper.Map<ApoderadoDTO>(apoderado);
            return CreatedAtRoute("ObtenerApoderadoV2", new { id = apoderado.Id }, apoderadoDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, 
            [FromForm] ApoderadoCreacionDTOConFoto apoderadoCreacionDTO)
        {
            var existeApoderado = await context.Apoderados.AnyAsync(x => x.Id == id);
            if (!existeApoderado)
            {
                return NotFound();
            }

            var apoderado = mapper.Map<Apoderado>(apoderadoCreacionDTO);
            apoderado.Id = id;

            if (apoderadoCreacionDTO.Foto is not null)
            {
                var fotoActual = await context
                                .Apoderados.Where(x => x.Id == id)
                                .Select(x => x.Foto).FirstAsync();

                var url = await almacenadorArchivos.Editar(fotoActual, contenedor,
                    apoderadoCreacionDTO.Foto);
                apoderado.Foto = url;
            }

            context.Update(apoderado);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);
            return NoContent();
        }

        //[HttpPatch("{id:int}")]
        //public async Task<ActionResult> Patch(int id, JsonPatchDocument<ApoderadoPatchDTO> patchDoc)
        //{
        //    if (patchDoc is null)
        //    {
        //        return BadRequest();
        //    }

        //    var apoderadoDB = await context.Apoderados.FirstOrDefaultAsync(x => x.Id == id);
        //    if (apoderadoDB is null)
        //    {
        //        return NotFound();
        //    }

        //    var apoderadoPatchDTO = mapper.Map<ApoderadoPatchDTO>(apoderadoDB);
        //    patchDoc.ApplyTo(apoderadoPatchDTO, ModelState);
        //    var esValido = TryValidateModel(apoderadoPatchDTO);
        //    if (!esValido)
        //    {
        //        return ValidationProblem();
        //    }

        //    mapper.Map(apoderadoPatchDTO, apoderadoDB);
        //    await context.SaveChangesAsync();
        //    await outputCacheStore.EvictByTagAsync(cache, default);
        //    return NoContent();
        //}

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var apoderado = await context.Apoderados.FirstOrDefaultAsync(x => x.Id == id);
            if (apoderado is null)
            {
                return NotFound();
            }
            context.Remove(apoderado);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cache, default);
            await almacenadorArchivos.Borrar(apoderado.Foto, contenedor);
            return NoContent();
        }
    }
}
