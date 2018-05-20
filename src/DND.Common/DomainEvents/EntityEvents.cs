using DND.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.DomainEvents
{
    public class EntityInsertedEvent<T> : IDomainEvent
        where T : IBaseEntity
    {
        public T Entity { get; }
        public string CreatedBy { get; }

        public EntityInsertedEvent(T entity, string createdBy)
        {
            Entity = entity;
            CreatedBy = createdBy;
        }
    }

    public class EntityUpdatedEvent<T> : IDomainEvent
        where T : IBaseEntity
    {
        public T Entity { get; }
        public string UpdatedBy { get; }

        public EntityUpdatedEvent(T entity, string updatedBy)
        {
            Entity = entity;
            UpdatedBy = updatedBy;
        }
    }

    public class EntityDeletedEvent<T> : IDomainEvent
        where T : IBaseEntity
    {
        public T Entity { get; }
        public string DeletedBy { get; }

        public EntityDeletedEvent(T entity, string deletedBy)
        {
            Entity = entity;
            DeletedBy = deletedBy;
        }
    }

    public class EntityPropertyUpdatedEvent<T> : IDomainEvent
        where T : IBaseEntity
    {
        public T Entity { get; }
        public Dictionary<string, OldAndNewValue> OldAndNewValues { get; }
        public string PropertyName { get; }
        public OldAndNewValue PropertyOldAndNewValue { get; }
        public string UpdatedBy { get; }

        public EntityPropertyUpdatedEvent(T entity, string updatedBy, Dictionary<string, OldAndNewValue> oldAndNewValues, string propertyName, OldAndNewValue propertyOldAndNewValue)
        {
            UpdatedBy = updatedBy;
            Entity = entity;
            OldAndNewValues = oldAndNewValues;
            PropertyName = propertyName;
            PropertyOldAndNewValue = propertyOldAndNewValue;
        }
    }

    public class OldAndNewValue
    {
        public string PropertyName { get; }
        public object OldValue { get; }
        public object NewValue { get; }

        public OldAndNewValue(string propertyName, object oldValue, object newValue)
        {
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public bool HasBeenUpdated
        {
            get
            {
                return !Equals(OldValue, NewValue);
            }
        }
    }
}
