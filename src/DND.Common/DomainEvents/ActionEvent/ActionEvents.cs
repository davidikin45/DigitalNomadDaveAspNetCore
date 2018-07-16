using AutoMapper;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.DomainEvents.ActionEvent
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
            var mapping = mapper.ConfigurationProvider.GetAllTypeMaps().Where(m => m.SourceType == dtoType && typeof(IBaseEntity).IsAssignableFrom(m.DestinationType)).FirstOrDefault();
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

            if (entityType is IBaseEntity)
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

        public IDomainActionEvent CreateEntityActionEvent(string action, dynamic args, object entity, string triggeredBy)
        {
            IDomainActionEvent actionEvent = null;

            if (entity is IBaseEntity)
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
