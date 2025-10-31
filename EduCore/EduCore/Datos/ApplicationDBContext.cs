using EduCore.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduCore.Datos
{
    public class ApplicationDBContext : IdentityDbContext<Usuario>
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);
        }

        //Este será el nombre de la tabla
        public DbSet<Apoderado> Apoderados { get; set; }
        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<ApoderadoAlumno> ApoderadoAlumnos { get; set; }
    }
}
