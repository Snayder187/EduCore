using AutoMapper;
using EduCore.Datos;
using EduCore.DTOs;
using EduCore.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduCore.Controllers
{
    [ApiController]
    [Route("api/matricula")]
    public class MatriculaController: ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public MatriculaController(ApplicationDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        //[HttpGet]
        //public async Task<IEnumerable<MatriculaDTO>> Get()
        //{
        //    var matriculas = await context.Matriculas
        //        .Include(x => x.Alumno)
        //        .Include(x => x.Curso)
        //        .ToListAsync();
        //    var matriculaDTO = mapper.Map<IEnumerable<MatriculaDTO>>(matriculas);
        //    return matriculaDTO;
        //}

        //[HttpGet("{id:int}", Name = "ObtenerMatricula")]
        //public async Task<ActionResult<MatriculaConAlumnoDTO>> Get(int id)
        //{
        //    var matricula = await context.Matriculas
        //        .Include(x => x.Alumno)
        //        .Include(x => x.Curso)
        //        .FirstOrDefaultAsync(x => x.Id == id);
        //    if (matricula is null)
        //    {
        //        return NotFound();
        //    }
        //    var matriculaDTO = mapper.Map<MatriculaConAlumnoDTO>(matricula);
        //    return matriculaDTO;
        //}

        //[HttpPost]
        //public async Task<ActionResult> Post(MatriculaCreacionDTO matriculaCreacionDTO)
        //{
        //    var matricula = mapper.Map<Matricula>(matriculaCreacionDTO);
        //    var existeAlumno = await context.Alumnos.AnyAsync(x => x.Id == matricula.AlumnoId);
        //    var existeCurso = await context.Cursos.AnyAsync(x => x.Id == matricula.CursoId);
            
        //    if (!existeAlumno)
        //    {
        //        ModelState.AddModelError(nameof(matricula.AlumnoId), $"El alumno de id {matricula.AlumnoId} no existe");
        //        return ValidationProblem();
        //    }
        //    if (!existeCurso)
        //    {
        //        ModelState.AddModelError(nameof(matricula.CursoId), $"El curso de id {matricula.CursoId} no existe");
        //        return ValidationProblem();
        //    }
        //    context.Add(matricula);
        //    await context.SaveChangesAsync();
        //    var matriculaDTO = mapper.Map<MatriculaDTO>(matricula);
        //    return CreatedAtRoute("ObtenerMatricula", new { id = matricula.Id }, matriculaDTO);
        //}

        //[HttpPut("{id:int}")]
        //public async Task<ActionResult> Put(int id, MatriculaCreacionDTO matriculaCreacionDTO)
        //{
        //    var matricula = mapper.Map<Matricula>(matriculaCreacionDTO);
        //    matricula.Id = id;
        //    var existeAlumno = await context.Alumnos.AnyAsync(x => x.Id == matricula.AlumnoId);
        //    var existeCurso = await context.Cursos.AnyAsync(x => x.Id == matricula.CursoId);
        //    if (!existeAlumno)
        //    {
        //        return BadRequest($"El alumno de id {matricula.AlumnoId} no existe");
        //    }
        //    if (!existeCurso)
        //    {
        //        return BadRequest($"El curso de id {matricula.CursoId} no existe");
        //    }

        //    context.Update(matricula);
        //    await context.SaveChangesAsync();
        //    return NoContent();
        //}

        //[HttpDelete]
        //public async Task<ActionResult> Delete(int id)
        //{
        //    var registroBorrado = await context.Matriculas.Where(x => x.Id == id).ExecuteDeleteAsync();
        //    if (registroBorrado == 0)
        //    {
        //        return NotFound();
        //    }
        //    return NoContent();
        //}
    }
}
