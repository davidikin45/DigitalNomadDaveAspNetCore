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
        Task<Result> HandlePreCommitAsync(T domainEvent);
        Task<Result> HandlePostCommitAsync(T domainEvent);
    }
}
