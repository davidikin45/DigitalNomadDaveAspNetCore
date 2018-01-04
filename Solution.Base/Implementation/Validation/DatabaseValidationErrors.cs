using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solution.Base.Interfaces.Validation;
using System.Data.Entity.Validation;

namespace Solution.Base.Implementation.Validation
{
    public class DatabaseValidationErrors : ValidationErrors
    {

        public DatabaseValidationErrors(IEnumerable<DbEntityValidationResult> errors) : base()
        {
            foreach (var err in errors.SelectMany(dbEntityValidationResult => dbEntityValidationResult.ValidationErrors))
            {
                Errors.Add(new PropertyError(err.PropertyName, err.ErrorMessage));
            }
        }
    }
}
