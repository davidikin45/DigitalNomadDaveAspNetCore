using DND.Common.DomainEvents;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Models
{
    public abstract class BaseEntityAggregateRoot<T> : BaseEntity<T>, IBaseEntityAggregateRoot, IBaseEntityConcurrencyAware where T : IEquatable<T>
    {
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();
        public virtual IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;

        //Deferred domain events
        //https://blogs.msdn.microsoft.com/cesardelatorre/2017/03/23/using-domain-events-within-a-net-core-microservice/
        protected virtual void AddDomainEvent(IDomainEvent newEvent)
        {
            _domainEvents.Add(newEvent);
        }

        public virtual void ClearEvents()
        {
            _domainEvents.Clear();
        }

        public void AddActionEvent(IDomainActionEvent actionEvent)
        {
            _domainEvents.Add(actionEvent);
        }

        //Optimistic Concurrency. Potentially ETags serve the same purpose
        public byte[] RowVersion { get; set; }
    }
}
