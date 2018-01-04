using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solution.Base.Interfaces.Validation;

namespace Solution.Base.Implementation.Validation
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
