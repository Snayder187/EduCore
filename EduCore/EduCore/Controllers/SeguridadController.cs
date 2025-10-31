using EduCore.Servicios;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace EduCore.Controllers
{
    [Route("api/seguridad")]
    [ApiController]
    public class SeguridadController : ControllerBase
    {
        private readonly IDataProtector protector;
        private readonly ITimeLimitedDataProtector protectorLimitadoPorTiempo;

        public IServicioHash ServicioHash { get; }

        public SeguridadController(IDataProtectionProvider protectionProvider, IServicioHash servicioHash)
        {
            protector = protectionProvider.CreateProtector("SeguridadController");
            protectorLimitadoPorTiempo = protector.ToTimeLimitedDataProtector();
            ServicioHash = servicioHash;
        }

        [HttpGet("hash")]
        public ActionResult Hash(string textoPlano)
        {
            var hash1 = ServicioHash.Hash(textoPlano);
            var hash2 = ServicioHash.Hash(textoPlano);
            var hash3 = ServicioHash.Hash(textoPlano, hash2.Sal);
            var resultado = new { textoPlano, hash1, hash2, hash3 };
            return Ok(resultado);
        }

        [HttpGet("encriptar-limitado-por-tiempo")]
        public ActionResult Encriptar(string textoPlano)
        {
            string textoCifrado = protectorLimitadoPorTiempo.Protect(textoPlano,
                lifetime: TimeSpan.FromSeconds(30));
            return Ok(new { textoCifrado });
        }

        [HttpGet("desencriptar-limitado-Por-Tiempo")]
        public ActionResult Desencriptar(string textoCifrado)
        {
            string textoPlano = protectorLimitadoPorTiempo.Unprotect(textoCifrado);
            return Ok(new { textoPlano });
        }

        //ENCRIPTAMIENTO LIMITADO
        //[HttpGet("encriptar")]
        //public ActionResult Encriptar(string textoPlano)
        //{
        //    string textoCifrado = protector.Protect(textoPlano);
        //    return Ok(new { textoCifrado });
        //}

        //[HttpGet("desencriptar")]
        //public ActionResult Desencriptar(string textoCifrado)
        //{
        //    string textoPlano = protector.Unprotect(textoCifrado);
        //    return Ok(new { textoPlano });
        //}
    }
}
