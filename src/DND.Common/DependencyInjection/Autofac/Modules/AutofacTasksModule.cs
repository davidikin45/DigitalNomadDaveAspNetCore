using Autofac;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DND.Common.Automapper;
using DND.Common.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.DependencyInjection.Autofac.Modules
{
    public class AutofacTasksModule : Module
    {

        public string[] Paths;
        public Func<string, Boolean> Filter;

        protected override void Load(ContainerBuilder builder)
        {
            var tasks = new List<Type>();
            tasks.Add(typeof(IRunAtStartup));
            tasks.Add(typeof(IRunOnEachRequest));
            tasks.Add(typeof(IRunOnError));
            tasks.Add(typeof(IRunAfterEachRequest));

            var added = new HashSet<string>();

            foreach (string path in Paths)
            {
                var assemblies = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly)
                              .Where(Filter)
                              .Select(System.Reflection.Assembly.LoadFrom);

                foreach (System.Reflection.Assembly assembly in assemblies)
                {
                    var types = assembly.GetTypes()
                              .Where(p => p.GetInterfaces().Intersect(tasks).Count() > 0)
                              .Select(p => p);

                    //  #4
                    foreach (var type in types)
                    {
                        foreach (var inter in type.GetInterfaces().Intersect(tasks))
                        {
                            if (!type.IsAbstract)
                            {
                                if (!type.IsGenericType && !added.Contains(type.FullName+inter.FullName))
                                {
                                    added.Add(type.FullName+inter.FullName);
                                    builder.RegisterType(type).As(inter);
                                }
                            }
                        }                      
                    
                    }
                }

            }
        }
    }


}
