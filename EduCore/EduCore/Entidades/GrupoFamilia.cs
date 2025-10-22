namespace EduCore.Entidades
{
    public class GrupoFamilia
    {
        public int Id { get; set; }
        public List<ApoderadoAlumno> Alumnos { get; set; } = [];
        public List<ApoderadoAlumno> Apoderados { get; set; } = [];
        public string Rol { get; set; } //Padre, Madre, Hijos
    }
}
