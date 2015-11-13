using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Contract.Validators
{
    public class ValidateObjectAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var context = new ValidationContext(value, null, null);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(value, context, results, validateAllProperties: true);

            if (!results.Any())
            {
                return ValidationResult.Success;
            }

            return new CompositeValidationResult(
                string.Format("Validation for {0} failed!", validationContext.DisplayName),
                results);
        }
    }
}
