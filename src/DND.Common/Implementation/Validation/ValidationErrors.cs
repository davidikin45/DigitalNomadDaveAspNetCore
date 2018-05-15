using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DND.Common.Interfaces.Validation;

namespace DND.Common.Implementation.Validation
{
    public class ValidationErrors : Exception, IValidationErrors
    {
        public List<IBaseError> Errors { get; set; }
        public ValidationErrors()
        {
            Errors = new List<IBaseError>();
        }

        public ValidationErrors(IBaseError error) : this()
        {
            Errors.Add(error);
        }

    }
}
