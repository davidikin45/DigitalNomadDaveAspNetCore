using DND.Common.DomainEvents;
using DND.Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.DomainEvents
{
    public static class DomainEventsFilter
    {
        public static Func<Assembly, Boolean> AssemblyFilter;
    }

    public static class DomainEvents
    {
        public static List<Type> _handlers;

        static DomainEvents()
        {
            Init();
        }

        public static void Init()
        {
            if (_handlers == null)
            {
                if (!(DomainEventsFilter.AssemblyFilter is null))
                {
                    Init(DomainEventsFilter.AssemblyFilter);
                }
            }
        }

        private static void Init(Func<Assembly, Boolean> filterFunc)
        {
            var type = typeof(IDomainEventHandler<>);
            _handlers = AppDomain.CurrentDomain.GetAssemblies()
                .Where(filterFunc)
                .SelectMany(s => s.GetTypes())
                .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)))
                .ToList();
        }

        public static void DispatchPreCommit(IDomainEvent domainEvent)
        {
            if(_handlers != null)
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
                        handler.HandlePreCommit((dynamic)domainEvent);
                    }
                }
            }
        }

        public static void DispatchPostCommit(IDomainEvent domainEvent)
        {
            if (_handlers != null)
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
}
