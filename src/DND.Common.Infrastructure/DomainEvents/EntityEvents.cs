﻿using DND.Common.Infrastructure.Interfaces.DomainEvents;
using System.Collections.Generic;

namespace DND.Common.Infrastructure.DomainEvents
{
    public class EntityActionEvent<T> : IDomainActionEvent
  where T : class
    {
        public string Action { get; }
        public dynamic Args { get; }
        public T Entity { get; }
        public string TriggeredBy { get; }

        public EntityActionEvent(string action, dynamic args, T entity, string triggeredBy)
        {
            Action = action;
            Args = args;
            Entity = entity;
            TriggeredBy = triggeredBy;
        }

        public override bool Equals(object obj)
        {
            var other = obj as EntityInsertedEvent<T>;

            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != obj.GetType())
                return false;

            return other.Entity.Equals(Entity);
        }
    }

    public class EntityInsertedEvent<T> : IDomainEvent
        where T : class
    {
        public T Entity { get; }
        public string CreatedBy { get; }

        public EntityInsertedEvent(T entity, string createdBy)
        {
            Entity = entity;
            CreatedBy = createdBy;
        }

        public override bool Equals(object obj)
        {
            var other = obj as EntityInsertedEvent<T>;

            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != obj.GetType())
                return false;

            return other.Entity.Equals(Entity);
        }
    }

    public class EntityUpdatedEvent<T> : IDomainEvent
        where T : class
    {
        public T Entity { get; }
        public string UpdatedBy { get; }

        public EntityUpdatedEvent(T entity, string updatedBy)
        {
            Entity = entity;
            UpdatedBy = updatedBy;
        }

        public override bool Equals(object obj)
        {
            var other = obj as EntityUpdatedEvent<T>;

            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != obj.GetType())
                return false;

            return other.Entity.Equals(Entity);
        }
    }

    public class EntityDeletedEvent<T> : IDomainEvent
        where T : class
    {
        public T Entity { get; }
        public string DeletedBy { get; }

        public EntityDeletedEvent(T entity, string deletedBy)
        {
            Entity = entity;
            DeletedBy = deletedBy;
        }

        public override bool Equals(object obj)
        {
            var other = obj as EntityDeletedEvent<T>;

            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != obj.GetType())
                return false;

            return other.Entity.Equals(Entity);
        }
    }

    public class EntityPropertyUpdatedEvent<T> : IDomainEvent
        where T : class
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

        public override bool Equals(object obj)
        {
            var other = obj as EntityPropertyUpdatedEvent<T>;

            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != obj.GetType())
                return false;

            return other.Entity.Equals(Entity) 
                && other.PropertyName.Equals(PropertyName)
                && other.PropertyOldAndNewValue.OldValue.Equals(PropertyOldAndNewValue.OldValue)
                 && other.PropertyOldAndNewValue.NewValue.Equals(PropertyOldAndNewValue.NewValue);
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
