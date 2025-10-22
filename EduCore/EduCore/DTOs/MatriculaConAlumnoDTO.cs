namespace EduCore.DTOs
{
    public class MatriculaConAlumnoDTO: MatriculaDTO
    {
        public int AlumnoId { get; set; }
        public required string AlumnoNombre { get; set; }
    }
}
