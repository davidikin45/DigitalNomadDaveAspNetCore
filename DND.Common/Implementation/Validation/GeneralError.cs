using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DND.Common.Interfaces.Validation;

namespace DND.Common.Implementation.Validation
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
