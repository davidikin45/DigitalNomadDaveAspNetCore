using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using DND.Common.Interfaces.UnitOfWork;
using DND.Common.Enums;

namespace DND.Common.Interfaces.Models
{
    public interface IBaseEntity : IBaseObjectValidatable
    {
         object Id { get; set; }
         Task<IEnumerable<ValidationResult>> ValidateWithDbConnectionAsync(IBaseUnitOfWorkScope unitOfWork, ValidationMode mode);
    }

    public interface IBaseEntity<T> : IBaseEntity where T : IEquatable<T>
    {
       new T Id { get; set; }
    }
}
