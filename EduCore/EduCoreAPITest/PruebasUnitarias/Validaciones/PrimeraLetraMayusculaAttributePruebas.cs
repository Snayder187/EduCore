using EduCore.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace EduCoreAPITest.PruebasUnitarias.Validaciones
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributePruebas
    {
        [TestMethod]
        [DataRow("")]
        [DataRow("     ")]
        [DataRow(null)]
        [DataRow("Hector")]
        public void IsValid_RetornaExitoso_SiValueNoTieneLaPrimeraLetraMinuscula(string value)
        {
            //Prepación
            var primeraLetraMayusculaAttribute = new PrimeraLetraMayusculaAttribute();
            var validationContext = new ValidationContext(new object());

            //Prueba
            var resultado = primeraLetraMayusculaAttribute.GetValidationResult(value, validationContext);

            //Verificación
            Assert.AreEqual(expected: ValidationResult.Success, actual: resultado);
        }

        [TestMethod]
        [DataRow("hector")]
        public void IsValid_RetornaExitoso_SiValueTieneLaPrimeraLetraMinuscula(string value)
        {
            //Prepación
            var primeraLetraMayusculaAttribute = new PrimeraLetraMayusculaAttribute();
            var validationContext = new ValidationContext(new object());

            //Prueba
            var resultado = primeraLetraMayusculaAttribute.GetValidationResult(value, validationContext);

            //Verificación
            Assert.AreEqual(expected: "La primera letra debe ser mayúscula", actual: resultado!.ErrorMessage);
        }
    }
}
