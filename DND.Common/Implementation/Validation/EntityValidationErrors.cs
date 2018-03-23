using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DND.Common.Interfaces.Validation;
using System.Data.Entity.Validation;
using System.ComponentModel.DataAnnotations;

namespace DND.Common.Implementation.Validation
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
