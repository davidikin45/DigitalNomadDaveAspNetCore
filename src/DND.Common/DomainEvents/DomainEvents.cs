using DND.Common.DomainEvents;
using DND.Common.Implementation.Validation;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.UnitOfWork;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
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

    public class DomainEvents : IDomainEvents
    {
        public static List<Type> _handlers;
        public static bool HandlePostCommitEventsInProcess = false;

        IServiceProvider _serviceProvider;
        public DomainEvents(IServiceProvider serviceProvider = null)
        {
            _serviceProvider = serviceProvider;
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


        private Dictionary<Type, List<Type>> eventHandlerTypes = new Dictionary<Type, List<Type>>();

        private List<dynamic> GetEventHandlerInstances(IDomainEvent domainEvent)
        {
            List<dynamic> instances = new List<dynamic>();

            if(_serviceProvider != null)
            {
                var eventHandlerInterfaceType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
                var types = typeof(IEnumerable<>).MakeGenericType(eventHandlerInterfaceType);
                dynamic handlers = _serviceProvider.GetService(types);

                foreach (var handler in handlers)
                {
                    instances.Add(handler);
                }
            }

            //Keep track of the types for the post event
            if (!eventHandlerTypes.ContainsKey(domainEvent.GetType()))
            {
                eventHandlerTypes.Add(domainEvent.GetType(), new List<Type>());

                foreach (var handler in instances)
                {
                    eventHandlerTypes[domainEvent.GetType()].Add(handler.GetType());
                }
            }

            return instances;
        }

        private List<Type> GetEventHandlerTypes(IDomainEvent domainEvent)
        {
            if (!eventHandlerTypes.ContainsKey(domainEvent.GetType()))
            {
                GetEventHandlerInstances(domainEvent);
            }
            return eventHandlerTypes[domainEvent.GetType()];
        }

        //InProcess
        public async Task DispatchPreCommitAsync(IDomainEvent domainEvent)
        {
            List<dynamic> handlers = GetEventHandlerInstances(domainEvent);

            //pre commit events are atomic
            foreach (var handler in handlers)
            {
                Result result = await handler.HandlePreCommitAsync((dynamic)domainEvent);
                if (result.IsFailure)
                {
                    throw new Exception("Pre Commit Event Failed");
                }
            }
        }

        //HandlePostCommitEventsInProcess determines whether PostCommit Events are handled InProcess or by Hangfire
        public async Task DispatchPostCommitAsync(IDomainEvent domainEvent)
        {
            var eventHandlerTypes = GetEventHandlerTypes(domainEvent);

            //Each Post Commit Domain Event Handling is completely independent. By registering the event AND handler (rather than just the event) in hangfire we get the granularity of retrying at a event/handler level.
            foreach (Type handlerType in eventHandlerTypes)
            {
                if (HandlePostCommitEventsInProcess)
                {
                    try
                    {
                        await HandlePostCommitAsync(handlerType, domainEvent).ConfigureAwait(false);
                    }
                    catch
                    {
                        //Log InProcess Post commit event failed
                    }
                }
                else
                {
                    try
                    {
                        //Hangfire unfortunately uses System.Type.GetType to get job type. This only looks at the referenced assemblies of the web project and not the dynamic loaded plugins so need to
                        //proxy back through this common assembly.
                        var exp = GetExpression(typeof(IDomainEvents), handlerType.FullName, domainEvent);

                        MethodInfo method = typeof(BackgroundJob).GetMethods().Where(m => m.Name == "Enqueue").Last();
                        MethodInfo generic = method.MakeGenericMethod(typeof(IDomainEvents));
                        generic.Invoke(null, new object[] { exp });
                    }
                    catch
                    {
                        //Log Hangfire Post commit event Background enqueue failed
                    }
                }
            }
        }

        private LambdaExpression GetExpression(Type jobType, string handlerName, object domainEvent)
        {
            var parameterExp = Expression.Parameter(jobType, "type");
            MethodInfo method = jobType.GetMethod(nameof(HandlePostCommitAsync), BindingFlags.Instance | BindingFlags.Public);
            MethodInfo genericMethod = method.MakeGenericMethod(domainEvent.GetType());
            var handlerValue = Expression.Constant(handlerName, typeof(string));
            var domainEventValue = Expression.Constant(domainEvent, domainEvent.GetType());
            var handlePostCommitAsyncExp = Expression.Call(parameterExp, genericMethod, handlerValue, domainEventValue);

            return Expression.Lambda(handlePostCommitAsyncExp, parameterExp);
        }

        //Called from Hangfire
        public async Task HandlePostCommitAsync<T>(string type, T domainEvent) where T : IDomainEvent
        {
            Type handlerType = System.Type.GetType(type);
            if (handlerType == null)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    handlerType = assembly.GetType(type);
                    if (handlerType != null)
                    {
                        break;
                    }
                }
            }

            if (handlerType == null)
            {
                throw new Exception("Invalid handler type");
            }

            await HandlePostCommitAsync(handlerType, domainEvent).ConfigureAwait(false);
        }

        public async Task HandlePostCommitAsync(Type handlerType, IDomainEvent domainEvent)
        {
            dynamic handler = _serviceProvider.GetService(handlerType);
            Result result = await handler.HandlePostCommitAsync((dynamic)domainEvent);
            if (result.IsFailure)
            {
                throw new Exception("Post Commit Event Failed");
            }
        }
    }
}
