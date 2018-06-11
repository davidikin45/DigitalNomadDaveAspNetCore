using DND.Common.Extensions;
using DND.Common.Interfaces.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.DomainEvents
{
    public class DbContextDomainEventsEF6Adapter : BaseDbContextDomainEvents
    {
        private DbContext _dbContext;
        public DbContextDomainEventsEF6Adapter(DbContext dbContext, IDomainEvents domainEvents)
            : base(domainEvents)
        {
            _dbContext = dbContext;
        }

        protected override IEnumerable<object> GetDeletedEntities()
        {
            return _dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted).Select(x => x.Entity);
        }

        protected override IEnumerable<object> GetInsertedEntities()
        {
            return _dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Added).Select(x => x.Entity);
        }

        protected override IEnumerable<object> GetUpdatedEntities()
        {
            return _dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).Select(x => x.Entity);
        }

        protected override IEnumerable<object> GetUpdatedDeletedInsertedEntities()
        {
            return _dbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted || e.State == EntityState.Added).Select(x => x.Entity);
        }

        protected override Dictionary<object, List<IDomainEvent>> GetNewPropertyUpdatedEvents()
        {
            var entries = _dbContext.ChangeTracker.Entries().Where(x => (x.State == EntityState.Modified));
            var events = CreatePropertyUpdateEventsEF6(entries);

            if (events == null)
            {
                events = new Dictionary<object, List<IDomainEvent>>();
            }

            var allDomainEvents = precommitedPropertyUpdateEvents.Values.MergeLists();

            foreach (var entity in events)
            {
                entity.Value.RemoveAll(e => allDomainEvents.Contains(e));
            }

            return events;
        }

        protected Dictionary<object, List<IDomainEvent>> CreatePropertyUpdateEventsEF6(IEnumerable<DbEntityEntry> updatedEntries)
        {
            var dict = new Dictionary<object, List<IDomainEvent>>();
            foreach (var updatedEntry in updatedEntries.Where(e => e.Entity is IFirePropertyUpdatedEvents))
            {
                var properties = new Dictionary<string, OldAndNewValue>();
                var updatedProperties = new List<string>();

                //If we have the original in cache compare to these values, otherwise hit the Db to get the Db values.
                var dbValues = updatedEntry.OriginalValues ?? updatedEntry.GetDatabaseValues();
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
            if (a1 == null && a2 == null)
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
    }
}
