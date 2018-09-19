using DND.Common.Infrastructure.Interfaces.DomainEvents;
using System.Collections.Generic;

namespace DND.Common.Infrastrucutre.Interfaces.Domain
{
    public interface IEntityAggregateRoot : IEntity
    {
        void AddActionEvent(IDomainActionEvent actionEvent);
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        void ClearEvents();

    }
}
