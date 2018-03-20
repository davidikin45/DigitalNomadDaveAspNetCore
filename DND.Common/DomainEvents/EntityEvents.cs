using DND.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.DomainEvents
{
    public class EntityInsertedEvent<T> : IDomainEvent
        where T: IBaseEntity
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

}
