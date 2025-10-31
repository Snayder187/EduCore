using AutoMapper;
using Azure;
using EduCore.Datos;
using EduCore.DTOs;
using EduCore.Entidades;
using EduCore.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduCore.Controllers
{
    [ApiController]
    [Route("api/apoderados/{apoderadoId:int}/comentarios")]
    [Authorize]
    public class ComentarioController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IServiciosUsuarios serviciosUsuarios;

        public ComentarioController(ApplicationDBContext context, IMapper mapper,
            IServiciosUsuarios serviciosUsuarios)
        {
            this.context = context;
            this.mapper = mapper;
            this.serviciosUsuarios = serviciosUsuarios;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int apoderadoId)
        {
            var existeApoderado = await context.Apoderados.AnyAsync(x => x.Id == apoderadoId);
            if (!existeApoderado)
            {
                return NotFound();
            }
            var comentarios = await context.Comentarios
                .Include(x => x.Usuario)
                .Where(x => x.ApoderadoId == apoderadoId)
                .OrderByDescending(x => x.FechaPublicacion)
                .ToListAsync();
            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id}", Name = "ObtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> Get(Guid id)
        {
            var comentario = await context.Comentarios
                                    .Include(x => x.Usuario)
                                    .FirstOrDefaultAsync(x => x.Id == id);
            if (comentario is null)
            {
                return NotFound();
            }
            return mapper.Map<ComentarioDTO>(comentario);
        }

        [HttpPost]
        public async Task<ActionResult> Post(int apoderadoId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var existeApoderado = await context.Apoderados.AnyAsync(x => x.Id == apoderadoId);
            if (!existeApoderado)
            {
                return NotFound();
            }

            var usuario = await serviciosUsuarios.ObtenerUsuario();
            if (usuario is null)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.ApoderadoId = apoderadoId;
            comentario.FechaPublicacion = DateTime.UtcNow;
            comentario.UsuarioId = usuario.Id;
            context.Add(comentario);
            await context.SaveChangesAsync();
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return CreatedAtRoute("ObtenerComentario", new { id = comentario.Id, apoderadoId }, comentarioDTO);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(Guid id, int apoderadoId, JsonPatchDocument<ComentarioPatchDTO> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest();
            }
            var existeApoderado = await context.Apoderados.AnyAsync(x => x.Id == apoderadoId);
            if (!existeApoderado)
            {
                return NotFound();
            }

            var usuario = await serviciosUsuarios.ObtenerUsuario();
            if (usuario is null)
            {
                return NotFound();
            }

            var comentarioDB = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
            if (comentarioDB is null)
            {
                return NotFound();
            }

            if (comentarioDB.UsuarioId != usuario.Id)
            {
                return Forbid();
            }

            var comentarioPatchDTO = mapper.Map<ComentarioPatchDTO>(comentarioDB);
            patchDoc.ApplyTo(comentarioPatchDTO, ModelState);
            var esValido = TryValidateModel(comentarioPatchDTO);
            if (!esValido)
            {
                return ValidationProblem();
            }
            mapper.Map(comentarioPatchDTO, comentarioDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id, int apoderadoId)
        {
            var existenciaApoderado = await context.Apoderados.AnyAsync(x => x.Id == apoderadoId);
            if (!existenciaApoderado)
            {
                return NotFound();
            }

            var usuario = await serviciosUsuarios.ObtenerUsuario();
            if (usuario is null)
            {
                return NotFound();
            }

            var comentarioDB = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
            if (comentarioDB is null)
            {
                return NotFound();
            }
            if (comentarioDB.UsuarioId != usuario.Id)
            {
                return Forbid();
            }
            context.Remove(comentarioDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
