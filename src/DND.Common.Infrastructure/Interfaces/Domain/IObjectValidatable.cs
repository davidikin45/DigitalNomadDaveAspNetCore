using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Common.Infrastrucutre.Interfaces.Domain
{
    public interface IObjectValidatable : IValidatableObject
    {
        Boolean IsValid();
        IEnumerable<ValidationResult> Validate();
    }
}
