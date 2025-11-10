using EduCore.DTOs;
using EduCoreAPITest.Utilidades;
using System.Net;

namespace EduCoreAPITest.PruebasDeIntegracion.Controllers.V1
{
    [TestClass]
    public class AlumnosControllerPruebas: BasePruebas
    {
        private readonly string url = "/api/v1/alumnos";
        private string nombreBD = Guid.NewGuid().ToString();

        [TestMethod]
        public async Task Post_Devuelve400_CuandoApoderadosIdsEsVacio()
        {
            //Preparación
            var factory = ConstruirWebApplicationFactory(nombreBD);
            var cliente = factory.CreateClient();
            var alumnoCreacionDTO = new AlumnoCreacionDTO { Nombres = "Zhe", ApellidoPaterno = "Ramos", ApellidoMaterno = "Miranda" };

            //Prueba
            var respuesta = await cliente.PostAsJsonAsync(url, alumnoCreacionDTO);

            //Verificación
            Assert.AreEqual(expected: HttpStatusCode.BadRequest, actual: respuesta.StatusCode);
        }
    }
}
