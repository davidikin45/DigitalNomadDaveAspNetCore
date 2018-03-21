using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DND.Common.Interfaces.Models
{
    public interface IBaseEntity : IBaseObjectValidatable
    {
         object Id { get; set; }

    }

    public interface IBaseEntity<T> : IBaseEntity
    {
       new T Id { get; set; }
    }
}
