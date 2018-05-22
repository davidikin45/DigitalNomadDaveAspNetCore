using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.DomainEvents
{
    public interface IDbContextDomainEvents
    {
        Dictionary<object, List<IDomainEvent>> CreatePropertyUpdateEventsEF6(IEnumerable<DbEntityEntry> updatedEntries);
        Dictionary<object, List<IDomainEvent>> CreatePropertyUpdateEventsEFCore(IEnumerable<EntityEntry> updatedEntries);

        Task DispatchDomainEventsPreCommitAsync(
        IEnumerable<object> updatedObjects,
        Dictionary<object, List<IDomainEvent>> propertyUpdatedEvents,
        IEnumerable<object> deletedObjects,
        IEnumerable<object> insertedObjects);

        Task DispatchDomainEventsPostCommitAsync(
          IEnumerable<object> updatedObjects,
          Dictionary<object, List<IDomainEvent>> propertyUpdatedEvents,
          IEnumerable<object> deletedObjects,
          IEnumerable<object> insertedObjects);
    }
}
