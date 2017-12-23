using DND.Base.Interfaces.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DND.Base.Implementation.Models
{
    public abstract class BaseObjectValidatable : IBaseObjectValidatable
    {
        public Boolean IsValid()
        {
            return Validate().Count() == 0;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            var context = new ValidationContext(this);

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(
                this,
                context,
               validationResults,
               true);

            return validationResults;

        }

        public abstract IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
    }
}
