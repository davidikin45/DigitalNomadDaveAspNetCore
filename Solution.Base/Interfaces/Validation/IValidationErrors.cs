using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Interfaces.Validation
{
    public interface IValidationErrors
    {
        List<IBaseError> Errors { get; set; }
    }
}
