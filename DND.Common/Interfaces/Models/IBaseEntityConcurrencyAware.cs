using DND.Common.DomainEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.Models
{
    public interface IBaseEntityConcurrencyAware
    {
        byte[] RowVersion { get; set; }
    }
}
