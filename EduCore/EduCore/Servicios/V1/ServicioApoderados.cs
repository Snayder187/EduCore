using AutoMapper;
using EduCore.Datos;
using EduCore.DTOs;
using EduCore.Utilidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduCore.Servicios.V1
{
    public class ServicioApoderados : IServicioApoderados
    {
        private readonly ApplicationDBContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;

        public ServicioApoderados(ApplicationDBContext context,
            IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<ApoderadoDTO>> Get(PaginacionDTO paginacionDTO)
        {
            //throw new NotImplementedException(); //Para hacer caer un error Status 500
            var queryable = context.Apoderados.AsQueryable();
            await httpContextAccessor.HttpContext!.InsertarParametrosPaginacionEnCabecera(queryable);
            var apoderados = await queryable
                            .OrderBy(x => x.Nombres)
                            .Paginar(paginacionDTO).ToListAsync();
            var apoderadosDTO = mapper.Map<IEnumerable<ApoderadoDTO>>(apoderados);
            return apoderadosDTO;
        }
    }
}
