using System.Collections.Generic;
using System.Threading.Tasks;

namespace DND.Common.Infrastructure.Interfaces.DomainEvents
{
    public interface IDomainEvents
    {
        Task DispatchPreCommitAsync(IDomainEvent domainEvent);
        Task DispatchPostCommitAsync(IDomainEvent domainEvent);
        Task DispatchPostCommitBatchAsync(IEnumerable<IDomainEvent> domainEvent);
        Task HandlePostCommitAsync<T>(string handlerType, T domainEvent) where T : IDomainEvent;
    }
}
