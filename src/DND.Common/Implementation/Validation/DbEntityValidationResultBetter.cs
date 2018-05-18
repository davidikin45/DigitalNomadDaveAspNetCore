using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Validation
{
    public class DbEntityValidationResultBetter
    {
        public DbEntityValidationResultBetter(IEnumerable<DbValidationError> validationErrors)
        {
            ValidationErrors = new List<DbValidationError>();
            foreach (var error in validationErrors)
            {
                ValidationErrors.Add(error);
            }
        }

        public DbEntityValidationResultBetter()
        {
            ValidationErrors = new List<DbValidationError>();
        }

        public void AddModelError(string property, string errorMessage)
        {
            var error = new DbValidationError(property, errorMessage);
            ValidationErrors.Add(error);
        }

        public ICollection<DbValidationError> ValidationErrors { get; }
        public bool IsValid
        {
            get
            {
               return ValidationErrors.Count() == 0;
            }
        }
    }
}
