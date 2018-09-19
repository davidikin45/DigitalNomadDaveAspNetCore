using System.ComponentModel.DataAnnotations;

namespace DND.Common.Domain.ModelMetadata
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
