using DND.Common.DomainEvents;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.UnitOfWork;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        //InProcess
        public async static Task DispatchPreCommitAsync(IDomainEvent domainEvent)
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
                        await handler.HandlePreCommitAsync((dynamic)domainEvent);
                    }
                }
            }
        }

        //InProcess at the moment but hopefully can make it out of process
        public async static Task DispatchPostCommitAsync(IDomainEvent domainEvent)
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

                        //Would be good to send this off to hangfire as a background job.
                        //Hangfire unfortunately uses System.Type.GetType to get job type. This only looks at the referenced assemblies of the web project and not the dynamic loaded plugins :(
                        //var exp = GetExpression(handlerType, domainEvent);

                        //MethodInfo method = typeof(BackgroundJob).GetMethods().Where(m => m.Name == "Enqueue").Last();
                        //MethodInfo generic = method.MakeGenericMethod(handlerType);
                        //generic.Invoke(null, new object[] { exp });

                        await handler.HandlePostCommitAsync((dynamic)domainEvent);
                    }
                }
            }
        }

        private static LambdaExpression GetExpression(Type handlerType, object domainEvent)
        {
            var parameterExp = Expression.Parameter(handlerType, "type");
            MethodInfo method = handlerType.GetMethod("HandlePostCommitAsync", BindingFlags.Instance | BindingFlags.Public);
            var someValue = Expression.Constant(domainEvent, domainEvent.GetType());
            var handlePostCommitAsyncExp = Expression.Call(parameterExp, method, someValue);

            return Expression.Lambda(handlePostCommitAsyncExp, parameterExp);
        }
    }
}
