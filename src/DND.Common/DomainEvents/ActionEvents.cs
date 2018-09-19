using AutoMapper;
using DND.Common.Infrastructure;
using DND.Common.Infrastructure.DomainEvents;
using DND.Common.Infrastructure.Interfaces.DomainEvents;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DND.Common.DomainEvents
{
    public class ActionEvents
    {
        public bool IsValidAction<T>(string action)
        {
           return GetActionsForDto(typeof(T)).ContainsKey(action);
        }

        public Dictionary<string, List<string>> GetActionsForDto(Type dtoType)
        {
            Dictionary<string, List<string>> actions = new Dictionary<string, List<string>>();

            var mapper = HttpContext.GetInstance<IMapper>();
            var mapping = mapper.ConfigurationProvider.GetAllTypeMaps().Where(m => m.SourceType == dtoType && typeof(IEntity).IsAssignableFrom(m.DestinationType)).FirstOrDefault();
            if (mapping != null)
            {
                var entityType = mapping.DestinationType;
                actions = GetActions(entityType);
            }
            return actions;
        }

        public Dictionary<string, List<string>> GetActions(Type entityType)
        {
            Dictionary<string, List<string>> actions = new Dictionary<string, List<string>>();

            IDomainActionEvent actionEvent = null;

            if (typeof(IEntity).IsAssignableFrom(entityType))
            {
                Type genericType = typeof(EntityActionEvent<>);
                Type[] typeArgs = { entityType };
                Type constructed = genericType.MakeGenericType(typeArgs);
                actionEvent = (IDomainActionEvent)Activator.CreateInstance(constructed, null, null, null, null);
            }

            if (actionEvent != null)
            {
                var eventHandlerInterfaceType = typeof(IDomainEventHandler<>).MakeGenericType(actionEvent.GetType());
                var types = typeof(IEnumerable<>).MakeGenericType(eventHandlerInterfaceType);
                dynamic handlers = HttpContext.GetInstance(types);

                foreach (var handler in handlers)
                {
                    IDictionary<string, string> handleActions = (IDictionary<string, string>)handler.HandleActions;
                    foreach (var handleAction in handleActions)
                    {
                        if (!actions.ContainsKey(handleAction.Key))
                        {
                            actions.Add(handleAction.Key, new List<string>());
                        }

                        if (!actions[handleAction.Key].Contains(handleAction.Value))
                        {
                            actions[handleAction.Key].Add(handleAction.Value);
                        }
                    }
                }
            }

            return actions;
        }
    }
}
