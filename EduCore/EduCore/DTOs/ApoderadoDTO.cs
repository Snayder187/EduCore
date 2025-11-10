namespace EduCore.DTOs
{
    public class ApoderadoDTO : RecursoDTO
    {
        public int Id { get; set; }
        public required string Nombres { get; set; }
        public required string Apellidos { get; set; }
        public string? Foto { get; set; }
    }
}
