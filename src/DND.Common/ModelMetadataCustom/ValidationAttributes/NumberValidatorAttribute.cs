using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.ModelMetadataCustom.ValidationAttributes
{
    public class NumberValidatorAttribute : RangeAttribute
    {
        public NumberValidatorAttribute()
            :base(0, int.MaxValue)
        {
            //The field {0} must be between {1} and {2}.
            ErrorMessage = "The field {0} must be a number.";
        }
    }
}
