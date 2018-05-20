﻿using DND.Common.Interfaces.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.DomainEvents
{
    public static class DbContextDomainEvents
    {
        public static Dictionary<object, List<IDomainEvent>> CreatePropertyUpdateEventsEF6(IEnumerable<DbEntityEntry> updatedEntries)
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

                        properties.Add(propertyName, new OldAndNewValue(original, current));

                        if (!Equals(original, current))
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
                    object domainEvent = Activator.CreateInstance(constructed, updatedEntry.Entity, properties, propertyName, properties[propertyName]);
                    propertyUpdatedEvents.Add((IDomainEvent)domainEvent);
                }

                if (propertyUpdatedEvents.Count > 0)
                {
                    dict.Add(updatedEntry.Entity, propertyUpdatedEvents);
                }
            }

            return dict;
        }

        public static Dictionary<object, List<IDomainEvent>> CreatePropertyUpdateEventsEFCore(IEnumerable<EntityEntry> updatedEntries)
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

                        properties.Add(property.Name, new OldAndNewValue(original, current));

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
                    object domainEvent = Activator.CreateInstance(constructed, updatedEntry.Entity, properties, propertyName, properties[propertyName]);
                    propertyUpdatedEvents.Add((IDomainEvent)domainEvent);
                }

                if (propertyUpdatedEvents.Count > 0)
                {
                    dict.Add(updatedEntry.Entity, propertyUpdatedEvents);
                }
            }

            return dict;
        }

        public static void DispatchDomainEventsPreCommit(
       IEnumerable<object> updatedObjects,
       Dictionary<object, List<IDomainEvent>> propertyUpdatedEvents,
       IEnumerable<object> deletedObjects,
       IEnumerable<object> insertedObjects)
        {
            var updated = updatedObjects.Where(x => x is IBaseEntity).Cast<IBaseEntity>();
            var deleted = deletedObjects.Where(x => x is IBaseEntity).Cast<IBaseEntity>();
            var inserted = insertedObjects.Where(x => x is IBaseEntity).Cast<IBaseEntity>();

            foreach (var entity in updated)
            {
                Type genericType = typeof(EntityUpdatedEvent<>);
                Type[] typeArgs = { entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                object domainEvent = Activator.CreateInstance(constructed, entity);
                DomainEvents.DispatchPreCommit((IDomainEvent)domainEvent);

                //Property Update Events
                if (propertyUpdatedEvents != null && propertyUpdatedEvents.ContainsKey(entity))
                {
                    foreach (var propertyUpdateEvent in propertyUpdatedEvents[entity])
                    {
                        DomainEvents.DispatchPreCommit(propertyUpdateEvent);
                    }
                }
            }

            foreach (var entity in deleted)
            {
                Type genericType = typeof(EntityDeletedEvent<>);
                Type[] typeArgs = { entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                object domainEvent = Activator.CreateInstance(constructed, entity);
                DomainEvents.DispatchPreCommit((IDomainEvent)domainEvent);
            }

            foreach (var entity in inserted)
            {
                Type genericType = typeof(EntityInsertedEvent<>);
                Type[] typeArgs = { entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                object domainEvent = Activator.CreateInstance(constructed, entity);
                DomainEvents.DispatchPreCommit((IDomainEvent)domainEvent);
            }

            var all = updated.Concat(deleted).Concat(inserted);

            foreach (var entity in all)
            {
                if (entity is IBaseEntityAggregateRoot)
                {
                    var aggRootEntity = ((IBaseEntityAggregateRoot)entity);
                    var events = aggRootEntity.DomainEvents.ToArray();
                    foreach (var domainEvent in events)
                    {
                        DomainEvents.DispatchPreCommit(domainEvent);
                    }
                }
            }
        }

        public static void DispatchDomainEventsPostCommit(
            IEnumerable<object> updatedObjects,
            Dictionary<object, List<IDomainEvent>> propertyUpdatedEvents,
            IEnumerable<object> deletedObjects,
            IEnumerable<object> insertedObjects)
        {
            var updated = updatedObjects.Where(x => x is IBaseEntity).Cast<IBaseEntity>();
            var deleted = deletedObjects.Where(x => x is IBaseEntity).Cast<IBaseEntity>();
            var inserted = insertedObjects.Where(x => x is IBaseEntity).Cast<IBaseEntity>();

            foreach (var entity in updated)
            {
                Type genericType = typeof(EntityUpdatedEvent<>);
                Type[] typeArgs = { entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                object domainEvent = Activator.CreateInstance(constructed, entity);
                try
                {
                    DomainEvents.DispatchPostCommit((IDomainEvent)domainEvent);
                }
                catch
                {
                    //Post Commit Events are best effort
                    //Log exception
                }

                //Property Update Events
                if (propertyUpdatedEvents != null && propertyUpdatedEvents.ContainsKey(entity))
                {
                    foreach (var propertyUpdateEvent in propertyUpdatedEvents[entity])
                    {
                        try
                        {
                            DomainEvents.DispatchPostCommit(propertyUpdateEvent);
                        }
                        catch
                        {
                            //Post Commit Events are best effort
                            //Log exception
                        }
                    }
                }
            }

            foreach (var entity in deleted)
            {
                Type genericType = typeof(EntityDeletedEvent<>);
                Type[] typeArgs = { entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                object domainEvent = Activator.CreateInstance(constructed, entity);
                try
                {
                   DomainEvents.DispatchPostCommit((IDomainEvent)domainEvent);
                }
                catch
                {
                    //Post Commit Events are best effort
                    //Log exception
                }
            }

            foreach (var entity in inserted)
            {
                Type genericType = typeof(EntityInsertedEvent<>);
                Type[] typeArgs = { entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                object domainEvent = Activator.CreateInstance(constructed, entity);
                try
                {
                   DomainEvents.DispatchPostCommit((IDomainEvent)domainEvent);
                }
                catch
                {
                    //Post Commit Events are best effort
                    //Log exception
                }
            }

            var all = updated.Concat(deleted).Concat(inserted);

            foreach (var entity in all)
            {
                if (entity is IBaseEntityAggregateRoot)
                {
                    var aggRootEntity = ((IBaseEntityAggregateRoot)entity);
                    var events = aggRootEntity.DomainEvents.ToArray();
                    aggRootEntity.ClearEvents();
                    foreach (var domainEvent in events)
                    {
                        try
                        {
                           DomainEvents.DispatchPostCommit(domainEvent);
                        }
                        catch
                        {
                            //Post Commit Events are best effort and should not throw exception.
                            //Log exception
                        }
                    }
                }
            }
        }
    }
}