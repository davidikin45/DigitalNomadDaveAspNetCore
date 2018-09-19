using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DND.Common.Infrastructure.Helpers
{
    public static class ValidationHelper
    {
        public static IEnumerable<ValidationResult> ValidateObject(object o)
        {
            var context = new ValidationContext(o);

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                o,
                context,
               validationResults,
               true);

            return validationResults.Where(r => r != ValidationResult.Success);
        }
    }
}
