using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using DND.Common.Interfaces.Models;

using System.Linq.Expressions;
using DND.Common.Implementation.Validation;

namespace DND.Common.Implementation.Models
{
    public abstract class BaseObjectValidatable : IBaseObjectValidatable
    {
        public Boolean IsValid()
        {
            return Validate().Count() == 0;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return ValidationHelper.ValidateObject(this);
        }

        public abstract IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
    }
}
