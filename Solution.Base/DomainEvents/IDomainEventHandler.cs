using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.DomainEvents
{
    public interface IDomainEventHandler<T>
        where T : IDomainEvent
    {
        void HandlePreCommit(T domainEvent);
        void HandlePostCommit(T domainEvent);
    }
}
