using AutoMapper;
using EduCore.DTOs;
using EduCore.Entidades;

namespace EduCore.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            //APODERADO
            CreateMap<Apoderado, ApoderadoDTO>()
                .ForMember(dto => dto.Apellidos,
                    config => config.MapFrom(autor => MapearNombreYApellidoApoderado(autor)));

            CreateMap<Apoderado, ApoderadoConAlumnoDTO>()
               .ForMember(dto => dto.Apellidos,
                   config => config.MapFrom(autor => MapearNombreYApellidoApoderado(autor)));

            CreateMap<ApoderadoCreacionDTO, Apoderado>();
            CreateMap<ApoderadoCreacionDTOConFoto, Apoderado>()
                .ForMember(ent => ent.Foto, config => config.Ignore());
            CreateMap<Apoderado, ApoderadoPatchDTO>().ReverseMap();

            CreateMap<ApoderadoAlumno, AlumnoDTO>()
                .ForMember(dto => dto.Id, config => config.MapFrom(ent => ent.AlumnoId))
                .ForMember(dto => dto.Nombres, config => config.MapFrom(ent => ent.Alumno!.Nombres))
                .ForMember(dto => dto.Apellidos, config => config.MapFrom(ent => MapearNombreYApellidoAlumno(ent.Alumno!)));
                
            //ALUMNO
            CreateMap<Alumno, AlumnoDTO>();
            CreateMap<AlumnoCreacionDTO, Alumno>()
                .ForMember(ent => ent.Apoderados, config =>
                    config.MapFrom(dto => dto.ApoderadosIds.Select(id => new ApoderadoAlumno { ApoderadoId = id })));

            CreateMap<Alumno, AlumnoConApoderadosDTO>();
            CreateMap<ApoderadoAlumno, ApoderadoDTO>()
                .ForMember(dto => dto.Id, config => config.MapFrom(ent => ent.ApoderadoId))
                .ForMember(dto => dto.Nombres, config => config.MapFrom(ent => ent.Apoderado!.Nombres))
                .ForMember(dto => dto.Apellidos, config => config.MapFrom(ent => MapearNombreYApellidoApoderado(ent.Apoderado!)));

            CreateMap<AlumnoCreacionDTO, ApoderadoAlumno>()
                .ForMember(ent => ent.Alumno, 
                    config => config.MapFrom(dto => 
                        new Alumno { Nombres = dto.Nombres, ApellidoPaterno = dto.ApellidoPaterno, ApellidoMaterno = dto.ApellidoMaterno }));

            //COMENTARIO
            CreateMap<ComentarioCreacionDTO, Comentario>();
            CreateMap<ComentarioPatchDTO, Comentario>().ReverseMap();
            CreateMap<Comentario, ComentarioDTO>()
                .ForMember(dto => dto.UsuarioEmail, config => config.MapFrom(ent => ent.Usuario!.Email));


            //USUARIO
            CreateMap<Usuario, UsuarioDTO>();
        }

        private string MapearNombreYApellidoAlumno(Alumno alumno) => $"{alumno.ApellidoPaterno} {alumno.ApellidoMaterno}";
        private string MapearNombreYApellidoApoderado(Apoderado apoderado) => $"{apoderado.ApellidoPaterno} {apoderado.ApellidoMaterno}";
    }
}
