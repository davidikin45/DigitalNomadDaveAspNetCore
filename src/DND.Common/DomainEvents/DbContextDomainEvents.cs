using DND.Common.Interfaces.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Common.DomainEvents
{
    public class DbContextDomainEvents : IDbContextDomainEvents
    {
        private IDomainEvents _domainEvents;
        public DbContextDomainEvents(IDomainEvents domainEvents)
        {
            _domainEvents = domainEvents;
        }


        public Dictionary<object, List<IDomainEvent>> CreateEntityUpdatedEvents(IEnumerable<object> updatedObjects)
        {
            var dict = new Dictionary<object, List<IDomainEvent>>();
            var updated = updatedObjects.Where(x => x is IBaseEntity).Cast<IBaseEntity>();

            foreach (var entity in updated)
            {
                var events = new List<IDomainEvent>();
                Type genericType = typeof(EntityUpdatedEvent<>);
                Type[] typeArgs = { entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                string updatedBy = "Anonymous";
                if (entity is IBaseEntityAuditable)
                {
                    updatedBy = ((IBaseEntityAuditable)entity).UserModified;
                }
                IDomainEvent domainEvent = (IDomainEvent)Activator.CreateInstance(constructed, entity, updatedBy);
                events.Add(domainEvent);
                dict.Add(entity, events);
            }

            return dict;
        }

        public Dictionary<object, List<IDomainEvent>> CreatePropertyUpdateEventsEF6(IEnumerable<DbEntityEntry> updatedEntries)
        {
            var dict = new Dictionary<object, List<IDomainEvent>>();
            foreach (var updatedEntry in updatedEntries.Where(e => e.Entity is IFirePropertyUpdatedEvents))
            {
                var properties = new Dictionary<string, OldAndNewValue>();
                var updatedProperties = new List<string>();

                var dbValues = updatedEntry.GetDatabaseValues();
                if (dbValues != null)
                {
                    foreach (string propertyName in dbValues.PropertyNames)
                    {
                        var original = dbValues[propertyName];
                        var current = updatedEntry.CurrentValues[propertyName];

                        properties.Add(propertyName, new OldAndNewValue(propertyName, original, current));

                        if (!Equals(original, current) && (!(original is byte[]) || ((original is byte[]) && !ByteArrayCompare((byte[])original, (byte[])current))))
                        {
                            updatedProperties.Add(propertyName);
                        }
                    }
                }

                var propertyUpdatedEvents = new List<IDomainEvent>();
                foreach (string propertyName in updatedProperties)
                {
                    Type genericType = typeof(EntityPropertyUpdatedEvent<>);
                    Type[] typeArgs = { updatedEntry.Entity.GetType() };
                    Type constructed = genericType.MakeGenericType(typeArgs);
                    string updatedBy = "Anonymous";
                    if (updatedEntry.Entity is IBaseEntityAuditable)
                    {
                        updatedBy = ((IBaseEntityAuditable)updatedEntry.Entity).UserModified;
                    }
                    object domainEvent = Activator.CreateInstance(constructed, updatedEntry.Entity, updatedBy, properties, propertyName, properties[propertyName]);
                    propertyUpdatedEvents.Add((IDomainEvent)domainEvent);
                }

                if (propertyUpdatedEvents.Count > 0)
                {
                    dict.Add(updatedEntry.Entity, propertyUpdatedEvents);
                }
            }

            return dict;
        }

        private bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if(a1 == null && a2 ==null)
            {
                return true;
            }
            if (a1 == null)
            {
                return false;

            }
            if (a2 == null)
            {
                return false;

            }
            return StructuralComparisons.StructuralEqualityComparer.Equals(a1, a2);
        }

        public Dictionary<object, List<IDomainEvent>> CreatePropertyUpdateEventsEFCore(IEnumerable<EntityEntry> updatedEntries)
        {
            var dict = new Dictionary<object, List<IDomainEvent>>();
            foreach (var updatedEntry in updatedEntries.Where(e => e.Entity is IFirePropertyUpdatedEvents))
            {

                var properties = new Dictionary<string, OldAndNewValue>();
                var updatedProperties = new List<string>();

                var dbValues = updatedEntry.GetDatabaseValues();
                if (dbValues != null)
                {
                    foreach (IProperty property in dbValues.Properties)
                    {
                        var original = dbValues[property.Name];
                        var current = updatedEntry.CurrentValues[property.Name];

                        properties.Add(property.Name, new OldAndNewValue(property.Name, original, current));

                        if (!Equals(original, current))
                        {
                            updatedProperties.Add(property.Name);
                        }
                    }
                }

                var propertyUpdatedEvents = new List<IDomainEvent>();
                foreach (string propertyName in updatedProperties)
                {
                    Type genericType = typeof(EntityPropertyUpdatedEvent<>);
                    Type[] typeArgs = { updatedEntry.Entity.GetType() };
                    Type constructed = genericType.MakeGenericType(typeArgs);

                    string updatedBy = "";
                    if(updatedEntry.Entity is IBaseEntityAuditable)
                    {
                        updatedBy = ((IBaseEntityAuditable)updatedEntry.Entity).UserModified;
                    }

                    object domainEvent = Activator.CreateInstance(constructed, updatedEntry.Entity, updatedBy, properties, propertyName, properties[propertyName]);
                    propertyUpdatedEvents.Add((IDomainEvent)domainEvent);
                }

                if (propertyUpdatedEvents.Count > 0)
                {
                    dict.Add(updatedEntry.Entity, propertyUpdatedEvents);
                }
            }

            return dict;
        }

        public Dictionary<object, List<IDomainEvent>> CreateEntityDeletedEvents(IEnumerable<object> deletedObjects)
        {
            var dict = new Dictionary<object, List<IDomainEvent>>();
            var deleted = deletedObjects.Where(x => x is IBaseEntity).Cast<IBaseEntity>();

            foreach (var entity in deleted)
            {
                var events = new List<IDomainEvent>();
                Type genericType = typeof(EntityDeletedEvent<>);
                Type[] typeArgs = { entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                string deletedBy = "Anonymous";
                if (entity is IBaseEntityAuditable)
                {
                    deletedBy = ((IBaseEntityAuditable)entity).UserDeleted;
                }
                IDomainEvent domainEvent = (IDomainEvent)Activator.CreateInstance(constructed, entity, deletedBy);
                events.Add(domainEvent);
                dict.Add(entity, events);
            }

            return dict;
        }

        public Dictionary<object, List<IDomainEvent>> CreateEntityInsertedEvents(IEnumerable<object> insertedObjects)
        {
            var dict = new Dictionary<object, List<IDomainEvent>>();
            var inserted = insertedObjects.Where(x => x is IBaseEntity).Cast<IBaseEntity>();

            foreach (var entity in inserted)
            {
                var events = new List<IDomainEvent>();
                Type genericType = typeof(EntityInsertedEvent<>);
                Type[] typeArgs = { entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                string createdBy = "Anonymous";
                if (entity is IBaseEntityAuditable)
                {
                    createdBy = ((IBaseEntityAuditable)entity).UserCreated;
                }
                IDomainEvent domainEvent = (IDomainEvent)Activator.CreateInstance(constructed, entity, createdBy);
                events.Add(domainEvent);
                dict.Add(entity, events);
            }

            return dict;
        }

        public Dictionary<object, List<IDomainEvent>> CreateEntityDomainEvents(IEnumerable<object> updatedDeletedInsertedObjects)
        {
            var dict = new Dictionary<object, List<IDomainEvent>>();
            var updatedDeletedInserted = updatedDeletedInsertedObjects.Where(x => x is IBaseEntity).Cast<IBaseEntity>();

            foreach (var entity in updatedDeletedInserted)
            {
                var events = new List<IDomainEvent>();
                if (entity is IBaseEntityAggregateRoot)
                {
                    var aggRootEntity = ((IBaseEntityAggregateRoot)entity);
                    var entityEvents = aggRootEntity.DomainEvents.ToArray();
                    foreach (var domainEvent in events)
                    {
                        events.Add(domainEvent);
                    }
                    aggRootEntity.ClearEvents();
                }
                dict.Add(entity, events);
            }

            return dict;
        }

        //If you are handling the domain events right before committing the original transaction is because you want the side effects of those events to be included in the same transaction
        public async Task DispatchDomainEventsPreCommitAsync(
       Dictionary<object, List<IDomainEvent>> entityUpdatedEvents,
       Dictionary<object, List<IDomainEvent>> propertyUpdatedEvents,
       Dictionary<object, List<IDomainEvent>> entityDeletedEvents,
       Dictionary<object, List<IDomainEvent>> entityInsertedEvents,
       Dictionary<object, List<IDomainEvent>> entityDomainEvents
       )
        {
            foreach (var kvp in entityUpdatedEvents)
            {
                foreach (var domainEvent in kvp.Value)
                {
                    await _domainEvents.DispatchPreCommitAsync(domainEvent).ConfigureAwait(false);
                }

                //Property Update Events
                if (propertyUpdatedEvents != null && propertyUpdatedEvents.ContainsKey(kvp.Key))
                {
                    foreach (var propertyUpdateEvent in propertyUpdatedEvents[kvp.Key])
                    {
                        await _domainEvents.DispatchPreCommitAsync(propertyUpdateEvent).ConfigureAwait(false);
                    }
                }
            }

            foreach (var kvp in entityDeletedEvents)
            {
                foreach (var domainEvent in kvp.Value)
                {
                    await _domainEvents.DispatchPreCommitAsync(domainEvent).ConfigureAwait(false);
                }
            }

            foreach (var kvp in entityInsertedEvents)
            {
                foreach (var domainEvent in kvp.Value)
                {
                    await _domainEvents.DispatchPreCommitAsync(domainEvent).ConfigureAwait(false);
                }
            }

            foreach (var kvp in entityDomainEvents)
            {
                foreach (var domainEvent in kvp.Value)
                {
                    await _domainEvents.DispatchPreCommitAsync(domainEvent).ConfigureAwait(false);
                }
            }
        }

        //If you are handling the domain events after committing the original transaction is because you do not want the side effects of those events to be included in the same transaction. e.g sending an email
        public async Task DispatchDomainEventsPostCommitAsync(
       Dictionary<object, List<IDomainEvent>> entityUpdatedEvents,
       Dictionary<object, List<IDomainEvent>> propertyUpdatedEvents,
       Dictionary<object, List<IDomainEvent>> entityDeletedEvents,
       Dictionary<object, List<IDomainEvent>> entityInsertedEvents,
       Dictionary<object, List<IDomainEvent>> entityDomainEvents)
        {

            foreach (var kvp in entityUpdatedEvents)
            {
                foreach (var domainEvent in kvp.Value)
                {
                    try
                    {
                        await _domainEvents.DispatchPostCommitAsync(domainEvent).ConfigureAwait(false);
                    }
                    catch
                    {

                    }
                }

                //Property Update Events
                if (propertyUpdatedEvents != null && propertyUpdatedEvents.ContainsKey(kvp.Key))
                {
                    foreach (var propertyUpdateEvent in propertyUpdatedEvents[kvp.Key])
                    {
                        try
                        {
                            await _domainEvents.DispatchPostCommitAsync(propertyUpdateEvent).ConfigureAwait(false);
                        }
                        catch
                        {

                        }
                    }
                }
            }

            foreach (var kvp in entityDeletedEvents)
            {
                foreach (var domainEvent in kvp.Value)
                {
                    try
                    {
                        await _domainEvents.DispatchPostCommitAsync(domainEvent).ConfigureAwait(false);
                    }
                    catch
                    {

                    }
                }
            }

            foreach (var kvp in entityInsertedEvents)
            {
                foreach (var domainEvent in kvp.Value)
                {
                    try
                    {
                        await _domainEvents.DispatchPostCommitAsync(domainEvent).ConfigureAwait(false);
                    }
                    catch
                    {

                    }
                }
            }

            foreach (var kvp in entityDomainEvents)
            {
                foreach (var domainEvent in kvp.Value)
                {
                    try
                    {
                        await _domainEvents.DispatchPostCommitAsync(domainEvent).ConfigureAwait(false);
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
}
