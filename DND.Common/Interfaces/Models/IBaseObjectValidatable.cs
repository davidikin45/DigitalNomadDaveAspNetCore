using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.Models
{
    interface IBaseObjectValidatable : IValidatableObject
    {
        Boolean IsValid();
        IEnumerable<ValidationResult> Validate();
    }
}
