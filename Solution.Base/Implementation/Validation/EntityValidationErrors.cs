using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solution.Base.Interfaces.Validation;
using System.Data.Entity.Validation;
using System.ComponentModel.DataAnnotations;

namespace Solution.Base.Implementation.Validation
{
    public class EntityValidationErrors : ValidationErrors
    {
        public EntityValidationErrors(IEnumerable<ValidationResult> errors) : base()
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
