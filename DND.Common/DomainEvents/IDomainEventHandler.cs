using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.DomainEvents
{
    public interface IDomainEventHandler<T>
        where T : IDomainEvent
    {
        void HandlePreCommit(T domainEvent);
        void HandlePostCommit(T domainEvent);
    }
}
