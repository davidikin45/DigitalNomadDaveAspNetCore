using DND.Common.Implementation.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : BaseApplicationUser
    {
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
           var errors = new List<ValidationResult>();
            return errors;
        }
    }
}
