using EduCore.Entidades;
using EduCoreAPITest.Utilidades;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Headers;

namespace EduCoreAPITest.PruebasDeIntegracion.Controllers.V1
{
    [TestClass]
    public class ComentariosControllerPruebas : BasePruebas
    {
        private readonly string url = "/api/v1/apoderados/1/comentarios";
        private string nombreBD = Guid.NewGuid().ToString();

        private async Task CrearDataDePrueba()
        {
            var context = ConstruirContext(nombreBD);
            var apoderado = new Apoderado { Nombres = "Hector", ApellidoPaterno = "Ramos", ApellidoMaterno = "Santisteban" };
            context.Add(apoderado);
            await context.SaveChangesAsync();

            var alumno = new Alumno { Nombres = "Razer", ApellidoPaterno = "Ramos", ApellidoMaterno = "Miranda" };
            alumno.Apoderados.Add(new ApoderadoAlumno { Apoderado = apoderado });
            context.Add(alumno);
            await context.SaveChangesAsync();
        }

        [TestMethod]
        public async Task Delete_Devuelve204_CuandoUsuarioBorraSuPropioComentario()
        {
            // Preparación
            await CrearDataDePrueba();
            var factory = ConstruirWebApplicationFactory(nombreBD, ignorarSeguridad: false);
            var token = await CrearUsuario(nombreBD, factory);
            var context = ConstruirContext(nombreBD);
            var usuario = await context.Users.FirstAsync();
            var comentario = new Comentario
            {
                Cuerpo = "contenido",
                UsuarioId = usuario!.Id,
                ApoderadoId = 1
            };

            context.Add(comentario);
            await context.SaveChangesAsync();

            var cliente = factory.CreateClient();
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Prueba
            var respuesta = await cliente.DeleteAsync($"{url}/{comentario.Id}");

            // Verificación
            Assert.AreEqual(expected: HttpStatusCode.NoContent, actual: respuesta.StatusCode);
        }

        [TestMethod]
        public async Task Delete_Devuelve403_CuandoUsuarioIntentaBorrarElComentarioDeOtro()
        {
            // Preparación
            await CrearDataDePrueba();

            var factory = ConstruirWebApplicationFactory(nombreBD, ignorarSeguridad: false);
            var emailCreadorComentario = "creador-comentario@hotmail.com";
            await CrearUsuario(nombreBD, factory, [], emailCreadorComentario);

            var context = ConstruirContext(nombreBD);
            var usuarioCreadorComentario = await context.Users.FirstAsync();

            var comentario = new Comentario
            {
                Cuerpo = "contenido",
                UsuarioId = usuarioCreadorComentario!.Id,
                ApoderadoId = 1
            };

            context.Add(comentario);
            await context.SaveChangesAsync();

            var tokenUsuarioDistinto = await CrearUsuario(nombreBD, factory, [], "usuario-distinto@hotmail.com");

            var cliente = factory.CreateClient();
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenUsuarioDistinto);

            // Prueba
            var respuesta = await cliente.DeleteAsync($"{url}/{comentario.Id}");

            // Verificación
            Assert.AreEqual(expected: HttpStatusCode.Forbidden, actual: respuesta.StatusCode);
        }
    }
}
