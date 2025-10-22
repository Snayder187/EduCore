using Microsoft.EntityFrameworkCore;

namespace EduCore.Entidades
{
    [PrimaryKey(nameof(ApoderadoId), nameof(AlumnoId))]
    public class ApoderadoAlumno
    {
        public int ApoderadoId { get; set; }
        public int AlumnoId { get; set; }
        public int Orden { get; set; }
        public Apoderado? Apoderado { get; set; }
        public Alumno? Alumno { get; set; }
    }
}
