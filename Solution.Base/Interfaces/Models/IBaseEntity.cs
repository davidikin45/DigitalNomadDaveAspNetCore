using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Solution.Base.Interfaces.Models
{
    public interface IBaseEntity : IValidatableObject
    {
         object Id { get; set; }

        Boolean IsValid();
        IEnumerable<ValidationResult> Validate();
    }

    public interface IBaseEntity<T> : IBaseEntity
    {
       new T Id { get; set; }
    }
}
