using System;
using System.Collections.Generic;

namespace DND.Common.Infrastructure.Validation.Errors
{
    public class ValidationErrors : Exception, IValidationErrors
    {
        public List<IError> Errors { get; set; }
        public ValidationErrors()
        {
            Errors = new List<IError>();
        }

        public ValidationErrors(IError error) : this()
        {
            Errors.Add(error);
        }

    }
}
