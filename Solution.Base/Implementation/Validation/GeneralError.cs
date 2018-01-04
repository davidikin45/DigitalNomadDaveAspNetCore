using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solution.Base.Interfaces.Validation;

namespace Solution.Base.Implementation.Validation
{
    public class GeneralError : IBaseError
    {
        #region Implementation of IBaseError

        public string PropertyName { get { return string.Empty; } }
        public string PropertyExceptionMessage { get; set; }

        public GeneralError(string errorMessage)
        {
            this.PropertyExceptionMessage = errorMessage;
        }

        #endregion
    }
}
