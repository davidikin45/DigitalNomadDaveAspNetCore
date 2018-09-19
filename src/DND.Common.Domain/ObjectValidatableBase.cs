using DND.Common.Infrastructure.Helpers;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DND.Common.Domain
{
    public abstract class ObjectValidatableBase : IObjectValidatable
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
