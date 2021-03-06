﻿using Autofac;
using DND.Common.Infrastructure.Interfaces.Data;
using System;
using System.IO;
using System.Linq;


namespace DND.Common.DependencyInjection.Autofac.Modules
{
    public class AutofacDbContextFactoryModule : Module
    {
        public string[] Paths;
        public Func<string, Boolean> Filter;

        protected override void Load(ContainerBuilder builder)
        {
            foreach (string path in Paths)
            {
                var assemblies = Directory.EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                              .Where(file => new[] { ".dll", ".exe" }.Any(file.ToLower().EndsWith))
                              .Where(Filter)
                              .Select(System.Reflection.Assembly.LoadFrom);

                foreach (System.Reflection.Assembly assembly in assemblies)
                {
                    var types = assembly.GetTypes()
                              .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(IDbContextFactory<>)))
                              .Select(p => p);

                    //  #4
                    foreach (var type in types)
                    {
                        if (!type.IsAbstract)
                        {
                            if (!type.IsGenericType)
                            {
                                builder.RegisterType(type).As(typeof(IDbContextAbstractFactory));
                            }
                        }
                    }
                }

            }
        }
    }


}
