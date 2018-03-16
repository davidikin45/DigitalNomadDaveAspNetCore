using Solution.Base.DomainEvents;
using Solution.Base.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.DomainEvents
{
    public static class DomainEvents
    {
        public static List<Type> _handlers;

        static DomainEvents()
        {
            if(_handlers is null)
            {
                Init();
            }
        }

        public static void Init()
        {
            var type = typeof(IDomainEventHandler<>);
            _handlers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)))
                .ToList();
        }

        public static void DispatchPreCommit(IDomainEvent domainEvent)
        {
            foreach (Type handlerType in _handlers)
            {
                bool canHandleEvent = handlerType.GetInterfaces()
                    .Any(x => x.IsGenericType
                    && x.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)
                    && x.GenericTypeArguments[0] == domainEvent.GetType());

                if(canHandleEvent)
                {
                    dynamic handler = StaticProperties.HttpContextAccessor.HttpContext.RequestServices.GetService(handlerType);
                    handler.HandlePreCommit((dynamic)domainEvent);
                }
            }
        }

        public static void DispatchPostCommit(IDomainEvent domainEvent)
        {
            foreach (Type handlerType in _handlers)
            {
                bool canHandleEvent = handlerType.GetInterfaces()
                    .Any(x => x.IsGenericType
                    && x.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)
                    && x.GenericTypeArguments[0] == domainEvent.GetType());

                if (canHandleEvent)
                {
                    dynamic handler = StaticProperties.HttpContextAccessor.HttpContext.RequestServices.GetService(handlerType);
                    handler.HandlePostCommit((dynamic)domainEvent);
                }
            }
        }
    }
}
