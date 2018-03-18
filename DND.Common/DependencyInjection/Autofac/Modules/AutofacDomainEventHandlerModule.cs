using Autofac;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DND.Common.Automapper;
using DND.Common.DomainEvents;
using DND.Common.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.DependencyInjection.Autofac.Modules
{
    public class AutofacDomainEventHandlerModule : Module
    {

        public string[] Paths;
        public Func<string, Boolean> Filter;

        protected override void Load(ContainerBuilder builder)
        {
            var tasks = new List<Type>();
            tasks.Add(typeof(IDomainEventHandler<>));

            foreach (string path in Paths)
            {
                var assemblies = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly)
                              .Where(Filter)
                              .Select(System.Reflection.Assembly.LoadFrom);

                foreach (System.Reflection.Assembly assembly in assemblies)
                {
                    var types = assembly.GetTypes()
                              .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)))
                              .Select(p => p);

                    //  #4
                    foreach (var type in types)
                    {
                        if (!type.IsAbstract)
                        {
                            if (!type.IsGenericType)
                            {
                                builder.RegisterType(type).As(type);
                            }
                        }
                    }
                }

            }
        }
    }


}
