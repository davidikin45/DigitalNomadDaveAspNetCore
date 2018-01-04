using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solution.Base.Interfaces.Validation;
using System.Data.Entity.Validation;

namespace Solution.Base.Implementation.Validation
{
    public class ServiceValidationErrors : ValidationErrors
    {
        public ServiceValidationErrors() : base()
        {
           
        }

        public ServiceValidationErrors(IBaseError error) : base(error)
        {
           
        }
    }
}
