using EduCore.DTOs;
using EduCore.Entidades;
using EduCoreAPITest.Utilidades;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace EduCoreAPITest.PruebasDeIntegracion.Controllers.V1
{
    [TestClass]
    public class ApoderadosControllerPruebas : BasePruebas
    {
        private static readonly string url = "/api/v1/apoderado";
        private string nombreBD = Guid.NewGuid().ToString();

        [TestMethod]
        public async Task Get_Devuelve404_CuandoApoderadoNoExiste()
        {
            // Preparación
            var factory = ConstruirWebApplicationFactory(nombreBD);
            var cliente = factory.CreateClient();

            // Prueba
            var respuesta = await cliente.GetAsync($"{url}/1");

            // Verificación
            var statusCode = respuesta.StatusCode;
            Assert.AreEqual(expected: HttpStatusCode.NotFound, actual: respuesta.StatusCode);
        }

        [TestMethod]
        public async Task Get_DevuelveApoderado_CuandoApoderadoExiste()
        {
            // Preparación
            var context = ConstruirContext(nombreBD);
            context.Apoderados.Add(new Apoderado() { Nombres = "Hector", ApellidoPaterno = "Ramos", ApellidoMaterno = "Santisteban" });
            context.Apoderados.Add(new Apoderado() { Nombres = "Zamira", ApellidoPaterno = "Cardenas", ApellidoMaterno = "Ferrer" });
            await context.SaveChangesAsync();

            var factory = ConstruirWebApplicationFactory(nombreBD);
            var cliente = factory.CreateClient();

            // Prueba
            Console.WriteLine($"Consultando URL: {url}/1");
            var respuesta = await cliente.GetAsync($"{url}/1");

            // Verificación
            respuesta.EnsureSuccessStatusCode();

            var apoderado = JsonSerializer.Deserialize<ApoderadoConAlumnoDTO>(
                await respuesta.Content.ReadAsStringAsync(), jsonSerializerOptions)!;

            Assert.AreEqual(expected: 1, apoderado.Id);
        }

        [TestMethod]
        public async Task Post_Devuelve404_CuandoUsuarioNoEstaAutenticado()
        {
            // Preparación
            var factory = ConstruirWebApplicationFactory(nombreBD, ignorarSeguridad: false);

            var apoderado = factory.CreateClient();
            var apoderadoCreacionDTO = new ApoderadoCreacionDTO
            {
                Nombres = "Hector",
                ApellidoPaterno = "Ramos",
                ApellidoMaterno = "Santisteban"
            };

            // Prueba
            var respuesta = await apoderado.PostAsJsonAsync(url, apoderadoCreacionDTO);

            // Verificación
            Assert.AreEqual(expected: HttpStatusCode.Unauthorized, actual: respuesta.StatusCode);
        }

        [TestMethod]
        public async Task Post_Devuelve403_CuandoUsuarioNoEsAdmin()
        {
            // Preparación
            var factory = ConstruirWebApplicationFactory(nombreBD, ignorarSeguridad: false);
            var token = await CrearUsuario(nombreBD, factory);

            var apoderado = factory.CreateClient();

            apoderado.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var apoderadoCreacionDTO = new ApoderadoCreacionDTO
            {
                Nombres = "Hector",
                ApellidoPaterno = "Ramos",
                ApellidoMaterno = "Santisteban"
            };

            // Prueba
            var respuesta = await apoderado.PostAsJsonAsync(url, apoderadoCreacionDTO);

            // Verificación
            Assert.AreEqual(expected: HttpStatusCode.Forbidden, actual: respuesta.StatusCode);
        }

        [TestMethod]
        public async Task Post_Devuelve201_CuandoUsuarioEsAdmin()
        {
            // Preparación
            var factory = ConstruirWebApplicationFactory(nombreBD, ignorarSeguridad: false);
            var claims = new List<Claim> { adminClaim };
            var token = await CrearUsuario(nombreBD, factory, claims);

            var apoderado = factory.CreateClient();

            apoderado.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var apoderadoCreacionDTO = new ApoderadoCreacionDTO
            {
                Nombres = "Hector",
                ApellidoPaterno = "Ramos",
                ApellidoMaterno = "Santisteban"
            };

            // Prueba
            var respuesta = await apoderado.PostAsJsonAsync(url, apoderadoCreacionDTO);

            // Verificación
            respuesta.EnsureSuccessStatusCode();
            Assert.AreEqual(expected: HttpStatusCode.Created, actual: respuesta.StatusCode);
        }
    }
}
