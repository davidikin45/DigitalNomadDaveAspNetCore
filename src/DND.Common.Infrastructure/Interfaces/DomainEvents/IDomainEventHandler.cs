using DND.Common.Infrastructure.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DND.Common.Infrastructure.Interfaces.DomainEvents
{
    public interface IDomainEventHandler<T>
        where T : IDomainEvent
    {
        IDictionary<string, string> HandleActions { get; }

        bool HandlePreCommitCondition(T domainEvent);
        Task<Result> HandlePreCommitAsync(T domainEvent);

        bool HandlePostCommitCondition(T domainEvent);
        Task<Result> HandlePostCommitAsync(T domainEvent);
    }
}
