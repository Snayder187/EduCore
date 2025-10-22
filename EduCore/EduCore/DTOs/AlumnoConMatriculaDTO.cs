namespace EduCore.DTOs
{
    public class AlumnoConMatriculaDTO: AlumnoDTO
    {
        public List<MatriculaDTO> Matriculas { get; set; } = [];
    }
}
