using EduCore.Entidades;
using Microsoft.EntityFrameworkCore;

namespace EduCore.Datos
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }

        //Este será el nombre de la tabla
        public DbSet<Apoderado> Apoderados { get; set; }
        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<ApoderadoAlumno> ApoderadoAlumnos { get; set; }
    }
}
