using AutoMapper;
using AutoMapper.EquivalencyExpression;
using DND.Common.Helpers;
using DND.Common.Infrastructure.Interfaces.Automapper;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using DND.Common.Infrastrucutre.Interfaces.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DND.Common.Automapper
{
    public class AutoMapperConfiguration
    {
        public AutoMapperConfiguration(IMapperConfigurationExpression mapperConfiguration, Func<Assembly, bool> assemblyFilter = null)
        {
            var target = Assembly.GetCallingAssembly();
            Configure(mapperConfiguration, assemblyFilter);
        }
        public void Configure(IMapperConfigurationExpression mapperConfiguration, Func<Assembly, bool> assemblyFilter = null)
        {
            //mapperConfiguration.AddProfile(new UserProfileMapping(mapperConfiguration));
            RegisterMappings(mapperConfiguration, assemblyFilter);
        }

        public static void RegisterMappings(IMapperConfigurationExpression cfg, Func<Assembly, bool> assemblyFilter = null)
        {

            Func<Assembly, bool> loadAllFilter = (x => true);

            var assembliesToLoad = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assemblyFilter ?? loadAllFilter)
                .Select(a => Assembly.Load(a.GetName()))
                .ToList();

            LoadMapsFromAssemblies(cfg, assembliesToLoad.ToArray());
        }

        public static void LoadMapsFromAssemblies(IMapperConfigurationExpression cfg, params Assembly[] assemblies)
        {
            var types = assemblies.SelectMany(a => a.GetExportedTypes()).ToArray();

            Load(cfg, types);
        }

        private static void Load(IMapperConfigurationExpression cfg, Type[] types)
        {
            LoadIMapFromMappings(cfg, types);
            LoadIMapToMappings(cfg, types);

            LoadCustomMappings(cfg, types);
        }

        private static void LoadCustomMappings(IMapperConfigurationExpression cfg, IEnumerable<Type> types)
        {
            var maps = (from t in types
                        where t.GetInterfaces().Count(i => typeof(IHaveCustomMappings).IsAssignableFrom(t) &&
                              !t.IsAbstract &&
                              !t.IsInterface) > 0
                        select (IHaveCustomMappings)Activator.CreateInstance(t)).ToArray();

            foreach (var map in maps)
            {
                map.CreateMappings(cfg);
            }
        }

        private static void LoadIMapFromMappings(IMapperConfigurationExpression cfg, IEnumerable<Type> types)
        {
            var maps = (from t in types
                        from i in t.GetInterfaces()
                        where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>) &&
                              !t.IsAbstract &&
                              !t.IsInterface
                        select new
                        {
                            Source = i.GetGenericArguments()[0],
                            Destination = t
                        }).ToArray();

            foreach (var map in maps)
            {
                //var mappingExpression = cfg.CreateMap(map.Source, map.Destination);

                var createMapMethod = typeof(IProfileExpression).GetMethod(nameof(IProfileExpression.CreateMap), new List<Type>() { }.ToArray()).MakeGenericMethod(map.Source, map.Destination);
                var mappingExpression = createMapMethod.Invoke(cfg, new List<Object>() { }.ToArray());

                if (typeof(IEntity).IsAssignableFrom(map.Source) && typeof(IDtoWithId).IsAssignableFrom(map.Destination))
                {
                    MapCollection(mappingExpression, map.Source, map.Destination);
                }
                else if (typeof(IDtoWithId).IsAssignableFrom(map.Source) && typeof(IEntity).IsAssignableFrom(map.Destination))
                {
                    MapCollection(mappingExpression, map.Source, map.Destination);
                }
            }
        }

        private static void LoadIMapToMappings(IMapperConfigurationExpression cfg, IEnumerable<Type> types)
        {
            var maps = (from t in types
                        from i in t.GetInterfaces()
                        where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapTo<>) &&
                              !t.IsAbstract &&
                              !t.IsInterface
                        select new
                        {
                            Destination = i.GetGenericArguments()[0],
                            Source = t
                        }).ToArray();

            foreach (var map in maps)
            {
                // var mappingExpression = cfg.CreateMap(map.Source, map.Destination);
                var createMapMethod = typeof(IProfileExpression).GetMethod(nameof(IProfileExpression.CreateMap), new List<Type>() { }.ToArray()).MakeGenericMethod(map.Source, map.Destination);
                var mappingExpression = createMapMethod.Invoke(cfg, new List<Object>() { }.ToArray());

                if (typeof(IEntity).IsAssignableFrom(map.Source) && typeof(IDtoWithId).IsAssignableFrom(map.Destination))
                {
                    MapCollection(mappingExpression, map.Source, map.Destination);
                }
                else if (typeof(IDtoWithId).IsAssignableFrom(map.Source) && typeof(IEntity).IsAssignableFrom(map.Destination))
                {
                    MapCollection(mappingExpression, map.Source, map.Destination);
                }
            }
        }

        private static void MapCollection(object mappingExpression, Type sourceType, Type destinationType)
        {
            var equalityComparisonMethod = typeof(EquivalentExpressions).GetMethod(nameof(EquivalentExpressions.EqualityComparison)).MakeGenericMethod(sourceType, destinationType);
            var equivalentExpression = LamdaHelper.SourceDestinationEquivalentExpressionById(sourceType, destinationType);
            equalityComparisonMethod.Invoke(null, new List<object>() { mappingExpression, equivalentExpression }.ToArray());
        }
    }
}
