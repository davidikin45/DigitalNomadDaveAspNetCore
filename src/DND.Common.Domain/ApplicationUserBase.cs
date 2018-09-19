using DND.Common.Infrastrucutre.Interfaces.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Common.Domain
{
    public abstract class ApplicationUserBase : IdentityUser, IEntity<string>, IEntityAuditable
    {
        public string Name { get; set; }
        object IEntity.Id
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

        public string UserOwner { get; set; }

        public string UserCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string UserModified { get; set; }

        //[NotMapped]
        public DateTime? DateDeleted { get; set; }
        //[NotMapped]
        public string UserDeleted { get; set; }

        public ApplicationUserBase()
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

        public abstract Task<IEnumerable<ValidationResult>> ValidateWithDbConnectionAsync(DbContext context, ValidationMode mode);
    }
}
