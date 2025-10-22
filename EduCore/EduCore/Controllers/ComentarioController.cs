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
    [Route("api/anuncios/{anuncioId:int}/comentarios")]
    public class ComentarioController: ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public ComentarioController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        //[HttpGet]
        //public async Task<ActionResult<List<ComentarioDTO>>> Get(int anuncioId)
        //{
        //    var existeAnuncio = await context.Anuncios.AnyAsync(x => x.Id == anuncioId);
        //    if (!existeAnuncio)
        //    {
        //        return NotFound();
        //    }
        //    var comentarios = await context.Comentarios
        //        .Where(x => x.AnuncioId == anuncioId)
        //        .OrderByDescending(x => x.FechaPublicacion)
        //        .ToListAsync();
        //    return mapper.Map<List<ComentarioDTO>>(comentarios);
        //}

        //[HttpGet("{id}", Name = "ObtenerComentario")]
        //public async Task<ActionResult<ComentarioDTO>> Get(Guid id)
        //{
        //    var comentario = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
        //    if (comentario is null)
        //    {
        //        return NotFound();
        //    }
        //    return mapper.Map<ComentarioDTO>(comentario);
        //}

        //[HttpPost]
        //public async Task<ActionResult> Post(int anuncioId, ComentarioCreacionDTO comentarioCreacionDTO)
        //{
        //    var existeAnuncio = await context.Anuncios.AnyAsync(x => x.Id == anuncioId);
        //    if (!existeAnuncio)
        //    {
        //        return NotFound();
        //    }
        //    var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
        //    comentario.AnuncioId = anuncioId;
        //    comentario.FechaPublicacion = DateTime.UtcNow;
        //    context.Add(comentario);
        //    await context.SaveChangesAsync();
        //    var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
        //    return CreatedAtRoute("ObtenerComentario", new { id = comentario.Id, anuncioId }, comentarioDTO);
        //}

        //[HttpPatch("{id}")]
        //public async Task<ActionResult> Patch(Guid id, int anuncioId, JsonPatchDocument<ComentarioPatchDTO> patchDoc)
        //{
        //    if (patchDoc is null)
        //    {
        //        return BadRequest();
        //    }
        //    var existeAnuncio = await context.Anuncios.AnyAsync(x => x.Id == anuncioId);
        //    if (!existeAnuncio)
        //    {
        //        return NotFound();
        //    }
        //    var comentarioDB = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
        //    if (comentarioDB is null)
        //    {
        //        return NotFound();
        //    }

        //    var comentarioPatchDTO = mapper.Map<ComentarioPatchDTO>(comentarioDB);
        //    patchDoc.ApplyTo(comentarioPatchDTO, ModelState);
        //    var esValido = TryValidateModel(comentarioPatchDTO);
        //    if (!esValido)
        //    {
        //        return ValidationProblem();
        //    }
        //    mapper.Map(comentarioPatchDTO, comentarioDB);
        //    await context.SaveChangesAsync();
        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<ActionResult> Delete(Guid id, int anuncioId)
        //{
        //    var existeAnuncio = await context.Anuncios.AnyAsync(x => x.Id == anuncioId);
        //    if (!existeAnuncio)
        //    {
        //        return NotFound();
        //    }

        //    var registroBorrados = await context.Comentarios.Where(x => x.Id == id).ExecuteDeleteAsync();
        //    if (registroBorrados == 0)
        //    {
        //        return NotFound();
        //    }
        //    return NoContent();
        //}
    }
}
