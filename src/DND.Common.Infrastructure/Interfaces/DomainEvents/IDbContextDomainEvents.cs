using System.Collections.Generic;
using System.Threading.Tasks;

namespace DND.Common.Infrastructure.Interfaces.DomainEvents
{
    public interface IDbContextDomainEvents
    {
        Task FirePreCommitEventsAsync();
        Task FirePostCommitEventsAsync();

        IEnumerable<object> GetNewDeletedEntities();
        IEnumerable<object> GetNewUpdatedEntities();
        IEnumerable<object> GetNewInsertedEntities();

        IEnumerable<object> GetPreCommittedDeletedEntities();
        IEnumerable<object> GetPreCommittedUpdatedEntities();
        IEnumerable<object> GetPreCommittedInsertedEntities();
    }
}
