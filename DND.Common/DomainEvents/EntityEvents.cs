using DND.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.DomainEvents
{
    public class InsertEntityEvent<T> : IDomainEvent
        where T: IBaseEntity
    {
        public T Entity { get; } 
        public InsertEntityEvent(T entity)
        {
            Entity = entity;
        }
    }

    public class UpdateEntityEvent<T> : IDomainEvent
        where T : IBaseEntity
    {
        public T Entity { get; }
        public UpdateEntityEvent(T entity)
        {
            Entity = entity;
        }
    }

    public class DeleteEntityEvent<T> : IDomainEvent
        where T : IBaseEntity
    {
        public T Entity { get; }
        public DeleteEntityEvent(T entity)
        {
            Entity = entity;
        }
    }

}
