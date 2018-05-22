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
        Dictionary<object, List<IDomainEvent>> CreateEntityUpdatedEvents(IEnumerable<object> updatedObjects);
        Dictionary<object, List<IDomainEvent>> CreatePropertyUpdateEventsEF6(IEnumerable<DbEntityEntry> updatedEntries);
        Dictionary<object, List<IDomainEvent>> CreatePropertyUpdateEventsEFCore(IEnumerable<EntityEntry> updatedEntries);
        Dictionary<object, List<IDomainEvent>> CreateEntityDeletedEvents(IEnumerable<object> deletedObjects);
        Dictionary<object, List<IDomainEvent>> CreateEntityInsertedEvents(IEnumerable<object> insertedObjects);
        Dictionary<object, List<IDomainEvent>> CreateEntityDomainEvents(IEnumerable<object> updatedDeletedInsertedObjects);

        Task DispatchDomainEventsPreCommitAsync(
        Dictionary<object, List<IDomainEvent>> entityUpdatedEvents,
       Dictionary<object, List<IDomainEvent>> propertyUpdatedEvents,
       Dictionary<object, List<IDomainEvent>> entityDeletedEvents,
       Dictionary<object, List<IDomainEvent>> entityInsertedEvents,
       Dictionary<object, List<IDomainEvent>> entityDomainEvents);

       Task DispatchDomainEventsPostCommitAsync(
       Dictionary<object, List<IDomainEvent>> entityUpdatedEvents,
       Dictionary<object, List<IDomainEvent>> propertyUpdatedEvents,
       Dictionary<object, List<IDomainEvent>> entityDeletedEvents,
       Dictionary<object, List<IDomainEvent>> entityInsertedEvents,
       Dictionary<object, List<IDomainEvent>> entityDomainEventss);
    }
}
