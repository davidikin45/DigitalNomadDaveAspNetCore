using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DND.Common.Infrastructure.Validation.Errors
{
    public class ObjectValidationErrors : ValidationErrors
    {
        public ObjectValidationErrors(IEnumerable<ValidationResult> errors) : base()
        { 
            foreach (var err in errors)
            {
                if (err.MemberNames.Count() > 0)
                {
                    foreach (var prop in err.MemberNames)
                    {
                        Errors.Add(new PropertyError(prop, err.ErrorMessage));
                    }
                }
                else
                {
                    Errors.Add(new GeneralError(err.ErrorMessage));
                }                  
            }
        }
    }
}
