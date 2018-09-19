using System.Collections.Generic;

namespace DND.Common.Infrastructure.Validation.Errors
{
    public interface IValidationErrors
    {
        List<IError> Errors { get; set; }
    }
}
