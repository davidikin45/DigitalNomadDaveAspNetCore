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
        public EntityInsertedEvent(T entity)
        {
            Entity = entity;
        }
    }

    public class EntityUpdatedEvent<T> : IDomainEvent
        where T : IBaseEntity
    {
        public T Entity { get; }
        public EntityUpdatedEvent(T entity)
        {
            Entity = entity;
        }
    }

    public class EntityDeletedEvent<T> : IDomainEvent
        where T : IBaseEntity
    {
        public T Entity { get; }
        public EntityDeletedEvent(T entity)
        {
            Entity = entity;
        }
    }

    public class EntityPropertyUpdatedEvent<T> : IDomainEvent
        where T : IBaseEntity
    {
        public T Entity { get; }
        public Dictionary<string, OldAndNewValue> OldAndNewValues { get; }
        public string PropertyName { get; }
        public OldAndNewValue PropertyOldAndNewValue { get; }

        public EntityPropertyUpdatedEvent(T entity, Dictionary<string, OldAndNewValue> oldAndNewValues, string propertyName, OldAndNewValue propertyOldAndNewValue)
        {
            Entity = entity;
            OldAndNewValues = oldAndNewValues;
            PropertyName = propertyName;
            PropertyOldAndNewValue = propertyOldAndNewValue;
        }
    }

    public class OldAndNewValue
    {
        public object OldValue { get; }
        public object NewValue { get; }

        public OldAndNewValue(object oldValue, object newValue)
        {

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
