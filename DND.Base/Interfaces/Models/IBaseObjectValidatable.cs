using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DND.Base.Interfaces.Model
{
    interface IBaseObjectValidatable : IValidatableObject
    {
        Boolean IsValid();
        IEnumerable<ValidationResult> Validate();
    }
}
