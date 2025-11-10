using Azure;
using EduCore.Controllers.V1;
using EduCore.DTOs;
using EduCore.Entidades;
using EduCore.Servicios;
using EduCore.Servicios.V1;
using EduCoreAPITest.Utilidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace EduCoreAPITest.PruebasUnitarias.Controllers.V1
{
    [TestClass]
    class ApoderadoControllerPrueba: BasePruebas
    {
        IAlmacenadorArchivos almacenadorArchivos = null!;
        ILogger<ApoderadoController> logger = null!;
        IOutputCacheStore outputCacheStore = null!;
        IServicioApoderados servicioApoderados = null!;
        private string nombreBD = Guid.NewGuid().ToString();
        private ApoderadoController controller = null!;

        [TestInitialize]
        public void Setuo()
        {
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            almacenadorArchivos = Substitute.For<IAlmacenadorArchivos>();
            logger = Substitute.For<ILogger<ApoderadoController>>();
            outputCacheStore = Substitute.For<IOutputCacheStore>();
            IServicioApoderados servicioApoderados = Substitute.For<IServicioApoderados>();

            controller = new ApoderadoController(context, mapper, almacenadorArchivos,
                logger, outputCacheStore, servicioApoderados);
        }

        [TestMethod]
        public async Task Get_Retorna404_CuandoApoderadoConIdNoExiste()
        {
            // Prueba
            var respuesta = await controller.Get(1);

            // Verificación
            var resultado = respuesta.Result as StatusCodeResult;
            Assert.AreEqual(expected: 404, actual: resultado!.StatusCode);
        }


        [TestMethod]
        public async Task Get_RetornaApoderado_CuandoApoderadoConIdExiste()
        {
            // Preparación
            var context = ConstruirContext(nombreBD);
            context.Apoderados.Add(new Apoderado { Nombres = "Hector", ApellidoPaterno = "Ramos", ApellidoMaterno = "Santisteban" });
            context.Apoderados.Add(new Apoderado { Nombres = "Krizia", ApellidoPaterno = "Diaz", ApellidoMaterno = "Ramos" });
            await context.SaveChangesAsync();

            // Prueba
            var respuesta = await controller.Get(1);

            // Verificación
            var resultado = respuesta.Value;
            Assert.AreEqual(expected: 1, actual: resultado!.Id);
        }

        [TestMethod]
        public async Task Get_RetornaApoderadoConAlumno_CuandoApoderadoTieneAlumnos()
        {
            // Preparación
            var context = ConstruirContext(nombreBD);
            var alumno1 = new Alumno { Nombres = "Raul", ApellidoPaterno = "Lozano", ApellidoMaterno = "Peña" };
            var alumno2 = new Alumno { Nombres = "Leslie", ApellidoPaterno = "León", ApellidoMaterno = "Barrientos" };

            var apoderado = new Apoderado()
            {
                Nombres = "Hector",
                ApellidoPaterno = "Ramos",
                ApellidoMaterno = "Santisteban",
                Alumnos = new List<ApoderadoAlumno>
                {
                    new ApoderadoAlumno{ Alumno = alumno1},
                    new ApoderadoAlumno{ Alumno = alumno2}
                }
            };

            await context.SaveChangesAsync();

            // Prueba
            var respuesta = await controller.Get(1);

            // Verificación
            var resultado = respuesta.Value;
            Assert.AreEqual(expected: 1, actual: resultado!.Id);
            Assert.AreEqual(expected: 2, actual: resultado.Alumnos.Count);
        }

        [TestMethod]
        public async Task Get_DebeLlamarGetDelServicioApoderados()
        {
            // Preparación
            var paginacionDTO = new PaginacionDTO(2, 3);

            // Prueba
            await controller.Get(paginacionDTO);

            // Verificación
            await servicioApoderados.Received(1).Get(paginacionDTO);
        }

        [TestMethod]
        public async Task Post_DebeCrearApoderado_CuandoEnviamosApoderado()
        {
            // Preparación
            var context = ConstruirContext(nombreBD);
            var nuevoApoderado = new ApoderadoCreacionDTO { Nombres = "nuevo", ApellidoPaterno = "apoderadoPaterno", ApellidoMaterno = "apoderadoMaterno" };

            // Prueba
            var respuesta = await controller.Post(nuevoApoderado);

            // Verificación
            var resultado = respuesta as CreatedAtRouteResult;
            Assert.IsNotNull(resultado);

            var contexto2 = ConstruirContext(nombreBD);
            var cantidad = await contexto2.Apoderados.CountAsync();
            Assert.AreEqual(expected: 1, actual: cantidad);
        }

        [TestMethod]
        public async Task Put_Retorna404_CuandoApoderadoNoExiste()
        {
            // Prueba
            var respuesta = await controller.Put(1, apoderadoCreacionDTO: null!);

            // Verificación
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404, resultado!.StatusCode);
        }

        private const string contenedor = "apoderados";
        private const string cache = "apoderados-obtener";

        [TestMethod]
        public async Task Put_ActualizarApoderado_CuandoEnviamosApoderadoSinFoto()
        {
            // Preparación
            var context = ConstruirContext(nombreBD);
            context.Apoderados.Add(new Apoderado
            {
                Nombres = "Hector",
                ApellidoPaterno = "Ramos",
                ApellidoMaterno = "Santisteban"
            });

            await context.SaveChangesAsync();

            var apoderadoCreacionDTO = new ApoderadoCreacionDTOConFoto
            {
                Nombres = "Hector2",
                ApellidoPaterno = "Ramos2",
                ApellidoMaterno = "Santisteban2"
            };

            // Prueba
            var respuesta = await controller.Put(1, apoderadoCreacionDTO);

            // Verificación
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado!.StatusCode);

            var context3 = ConstruirContext(nombreBD);
            var apoderadoActualizado = await context3.Apoderados.SingleAsync();
            Assert.AreEqual(expected: "Hector2", actual: apoderadoActualizado.Nombres);
            Assert.AreEqual(expected: "Ramos2", actual: apoderadoActualizado.ApellidoPaterno);
            Assert.AreEqual(expected: "Santisteban2", actual: apoderadoActualizado.ApellidoMaterno);
            await outputCacheStore.Received(1).EvictByTagAsync(cache, default);
            await almacenadorArchivos.DidNotReceiveWithAnyArgs().Editar(default, default!, default!);
        }

        [TestMethod]
        public async Task Put_ActualizarApoderado_CuandoEnviamosApoderadoConFoto()
        {
            // Preparación
            var context = ConstruirContext(nombreBD);
            var urlAnterior = "URL-1";
            var urlNueva = "URL-2";
            almacenadorArchivos.Editar(default, default!, default!).ReturnsForAnyArgs(urlNueva);

            context.Apoderados.Add(new Apoderado
            {
                Nombres = "Hector",
                ApellidoPaterno = "Ramos",
                ApellidoMaterno = "Santisteban",
                Foto = urlAnterior
            });

            await context.SaveChangesAsync();
            var formFile = Substitute.For<IFormFile>();

            var apoderadoCreacionDTO = new ApoderadoCreacionDTOConFoto
            {
                Nombres = "Hector2",
                ApellidoPaterno = "Ramos2",
                ApellidoMaterno = "Santisteban2",
                Foto = formFile
            };

            // Prueba
            var respuesta = await controller.Put(1, apoderadoCreacionDTO);

            // Verificación
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado!.StatusCode);

            var context3 = ConstruirContext(nombreBD);
            var apoderadoActualizado = await context3.Apoderados.SingleAsync();
            Assert.AreEqual(expected: "Hector2", actual: apoderadoActualizado.Nombres);
            Assert.AreEqual(expected: "Ramos2", actual: apoderadoActualizado.ApellidoPaterno);
            Assert.AreEqual(expected: "Santisteban2", actual: apoderadoActualizado.ApellidoMaterno);
            Assert.AreEqual(expected: urlNueva, actual: apoderadoActualizado.Foto);
            await outputCacheStore.Received(1).EvictByTagAsync(cache, default);
            await almacenadorArchivos.Received(1).Editar(urlAnterior, contenedor, formFile);
        }

        [TestMethod]
        public async Task Patch_Retornar404_CuandoPatchDocEsNulo()
        {
            // Prueba
            var respuesta = await controller.Patch(1, patchDoc: null!);

            // Verificación
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(400, resultado!.StatusCode);
        }

        [TestMethod]
        public async Task Patch_Retornar404_CuandoApoderadoNoExiste()
        {
            // Preparación
            var patchDoc = new JsonPatchDocument<ApoderadoPatchDTO>();

            // Prueba
            var respuesta = await controller.Patch(1, patchDoc: null!);

            // Verificación
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404, resultado!.StatusCode);
        }

        [TestMethod]
        public async Task Patch_RetornarValidationProblem_CuandoHayErrorDeValidacion()
        {
            // Preparación
            var context = ConstruirContext(nombreBD);
            context.Apoderados.Add(new Apoderado
            {
                Nombres = "Hector",
                ApellidoPaterno = "Ramos",
                ApellidoMaterno = "Santisteban"
            });

            await context.SaveChangesAsync();
            var objectValidator = Substitute.For<IObjectModelValidator>();
            controller.ObjectValidator = objectValidator;

            var mensajeDeError = "mensaje de error";
            controller.ModelState.AddModelError("", mensajeDeError);

            var patchDoc = new JsonPatchDocument<ApoderadoPatchDTO>();

            // Prueba
            var respuesta = await controller.Patch(1, patchDoc);

            // Verificación
            var resultado = respuesta as ObjectResult;
            var problemDetails = resultado!.Value as ValidationProblemDetails;
            Assert.IsNotNull(problemDetails);
            Assert.AreEqual(expected: 1, actual: problemDetails.Errors.Keys.Count);
            Assert.AreEqual(expected: mensajeDeError, actual: problemDetails.Errors.Values.First().First());
        }

        [TestMethod]
        public async Task Patch_ActualizaUnCampo_CuandoSeLeEnvieUnaOperacion()
        {
            // Preparación
            var context = ConstruirContext(nombreBD);
            context.Apoderados.Add(new Apoderado
            {
                Nombres = "Hector",
                ApellidoPaterno = "Ramos",
                ApellidoMaterno = "Santisteban",
                Foto = "URL-1"
            });

            await context.SaveChangesAsync();
            var objectValidator = Substitute.For<IObjectModelValidator>();
            controller.ObjectValidator = objectValidator;

            var patchDoc = new JsonPatchDocument<ApoderadoPatchDTO>();
            patchDoc.Operations.Add(new Microsoft.AspNetCore.JsonPatch.Operations.
                Operation<ApoderadoPatchDTO>("replace", "/nombres", null, "Hector2"));

            // Prueba
            var respuesta = await controller.Patch(1, patchDoc);

            // Verificación
            var resultado = respuesta as ObjectResult;
            Assert.AreEqual(expected: 204, resultado!.StatusCode);
            await outputCacheStore.Received(1).EvictByTagAsync(cache, default);

            var context2 = ConstruirContext(nombreBD);
            var apoderadoBD = await context2.Apoderados.SingleAsync();

            Assert.AreEqual(expected: "Hector2", apoderadoBD.Nombres);
            Assert.AreEqual(expected: "Ramos", apoderadoBD.ApellidoPaterno);
            Assert.AreEqual(expected: "Santisteban", apoderadoBD.ApellidoMaterno);
            Assert.AreEqual(expected: "URL-1", apoderadoBD.Foto);
        }

        [TestMethod]
        public async Task Delete_Retornar404_CuandoApoderadoNoExiste()
        {
            // Prueba
            var respuesta = await controller.Delete(1);

            // Verificación
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404, resultado!.StatusCode);
        }

        [TestMethod]
        public async Task Delete_BorraApoderado_CuandoApoderadoExiste()
        {
            // Preparación
            var urlFoto = "URL-1";
            var context = ConstruirContext(nombreBD);

            context.Apoderados.Add(new Apoderado { Nombres = "Hector1", ApellidoPaterno = "Ramos1", ApellidoMaterno = "Ramos1", Foto = urlFoto });
            context.Apoderados.Add(new Apoderado { Nombres = "Hector2", ApellidoPaterno = "Ramos2", ApellidoMaterno = "Ramos2" });

            await context.SaveChangesAsync();

            // Prueba
            var respuesta = await controller.Delete(1);

            // Verificación
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado!.StatusCode);

            var context2 = ConstruirContext(nombreBD);
            var cantidadApoderados = await context2.Apoderados.CountAsync();
            Assert.AreEqual(expected: 1, actual: cantidadApoderados);

            var apoderado2Existe = await context2.Apoderados.AnyAsync(x => x.Nombres == "Hector2");
            Assert.IsTrue(apoderado2Existe);

            await outputCacheStore.Received(1).EvictByTagAsync(cache, default);
            await almacenadorArchivos.Received(1).Borrar(urlFoto, contenedor);
        }
    }
}
