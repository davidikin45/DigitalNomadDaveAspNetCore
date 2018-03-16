using Solution.Base.DomainEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Interfaces.Models
{
    public interface IBaseEntityAggregateRoot : IBaseEntity
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get;  }
        void ClearEvents();

    }
}
