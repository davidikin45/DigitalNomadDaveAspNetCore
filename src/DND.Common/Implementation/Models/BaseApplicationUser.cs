using Microsoft.AspNetCore.Identity;
using DND.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DND.Common.Implementation.Models
{
    public abstract class BaseApplicationUser : IdentityUser, IBaseEntity<string>, IBaseEntityAuditable
    {
        public string Name { get; set; }
        object IBaseEntity.Id
        {
            get { return this.Id; }
            set { this.Id = (string)value; }
        }

        private DateTime? _dateCreated;
        public DateTime DateCreated
        {
            get { return _dateCreated ?? DateTime.UtcNow; }
            set { _dateCreated = value; }
        }

        public string UserCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string UserModified { get; set; }

        public BaseApplicationUser()
        {

        }

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
