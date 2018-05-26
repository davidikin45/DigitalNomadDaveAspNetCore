using DND.Common.Implementation.Validation;
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
        bool HandlePreCommitCondition(T domainEvent);
        Task<Result> HandlePreCommitAsync(T domainEvent);

        bool HandlePostCommitCondition(T domainEvent);
        Task<Result> HandlePostCommitAsync(T domainEvent);
    }
}
