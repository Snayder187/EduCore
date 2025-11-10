using EduCore.Controllers.V1;
using EduCore.DTOs;
using EduCoreAPITest.Utilidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OutputCaching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCoreAPITest.PruebasUnitarias.Controllers.V1
{
    [TestClass]
    public class AlumnosControllerPruebas: BasePruebas
    {
        [TestMethod]
        public async Task Get_RetornarCeroAlumnos_CuandoNoHayAlumnos()
        {
            // Preparación
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            IOutputCacheStore outputCacheStore = null!;

            var controller = new AlumnoController(context, mapper, outputCacheStore);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var paginacionDTO = new PaginacionDTO(1, 1);

            // Prueba
            var respuesta = await controller.Get(paginacionDTO);

            // Verificación
            Assert.AreEqual(expected: 0, actual: respuesta.Count());
        }
    }
}
