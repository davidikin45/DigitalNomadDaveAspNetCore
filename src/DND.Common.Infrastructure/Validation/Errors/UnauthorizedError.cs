using System;
using System.Collections.Generic;

namespace DND.Common.Infrastructure.Validation.Errors
{
    public class UnauthorizedErrors : Exception, IValidationErrors
    {
        public List<IError> Errors { get; set; }
        public UnauthorizedErrors()
        {
            Errors = new List<IError>();
        }

        public UnauthorizedErrors(IError error) : this()
        {
            Errors.Add(error);
        }
    }
}
