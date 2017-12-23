using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DND.Base.Interfaces.Model
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
