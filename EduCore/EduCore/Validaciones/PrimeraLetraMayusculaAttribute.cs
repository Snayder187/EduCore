using System.ComponentModel.DataAnnotations;

namespace EduCore.Validaciones
{
    public class PrimeraLetraMayusculaAttribute: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }
            var valueString = value.ToString()!;
            var primeraLetra = valueString[0].ToString();
            //var primeraLetra = value.ToString()![0].ToString(); //Esto es lo mismo que las 2 lineas de arriba
            if (primeraLetra != primeraLetra.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayúscula");
            }
            return ValidationResult.Success;
        }
    }
}
