using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Common.Infrastrucutre.Interfaces.Domain
{
    public interface IEntity : IObjectValidatable
    {
        object Id { get; set; }
        Task<IEnumerable<ValidationResult>> ValidateWithDbConnectionAsync(DbContext unitOfWork, ValidationMode mode);
    }

    public interface IEntity<T> : IEntity where T : IEquatable<T>
    {
        new T Id { get; set; }
    }
}
