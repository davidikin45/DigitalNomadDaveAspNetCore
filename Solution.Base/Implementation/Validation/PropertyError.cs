using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solution.Base.Interfaces.Validation;

namespace Solution.Base.Implementation.Validation
{
    public class PropertyError : IBaseError
    {
        public string PropertyName { get; set; }
        public string PropertyExceptionMessage { get; set; }
        public PropertyError(string propertyName, string errorMessage)
        {
            this.PropertyName = propertyName;
            this.PropertyExceptionMessage = errorMessage;
        }
    }
}
