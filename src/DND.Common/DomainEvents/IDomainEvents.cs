using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.DomainEvents
{
    public interface IDomainEvents
    {
        Task DispatchPreCommitAsync(IDomainEvent domainEvent);
        Task HandlePostCommitAsync<T>(string handlerType, T domainEvent) where T : IDomainEvent;
    }
}
