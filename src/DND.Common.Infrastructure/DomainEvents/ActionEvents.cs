using DND.Common.Infrastructure.Interfaces.DomainEvents;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using System;

namespace DND.Common.Infrastructure.DomainEvents
{
    public class ActionEvents
    {
        public IDomainActionEvent CreateEntityActionEvent(string action, dynamic args, object entity, string triggeredBy)
        {
            IDomainActionEvent actionEvent = null;

            if (entity is IEntity)
            {
                Type genericType = typeof(EntityActionEvent<>);
                Type[] typeArgs = { entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                actionEvent = (IDomainActionEvent)Activator.CreateInstance(constructed, action, args, entity, triggeredBy);
            }

            return actionEvent;
        }
    }
}
