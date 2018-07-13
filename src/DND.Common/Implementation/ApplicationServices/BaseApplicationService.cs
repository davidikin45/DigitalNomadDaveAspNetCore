using AutoMapper;
using AutoMapper.Configuration.Internal;
using AutoMapper.Extensions.ExpressionMapping;
using AutoMapper.Internal;
using AutoMapper.Mappers.Internal;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DND.Common.Implementation.ApplicationServices
{
    public abstract class BaseApplicationService : IBaseApplicationService
    {
        private readonly IFileSystemGenericRepositoryFactory _fileSystemGenericRepositoryFactory;
        public IMapper Mapper { get; }

        public BaseApplicationService(IMapper mapper)
        {
            Mapper = mapper;
        }

        public BaseApplicationService(IFileSystemGenericRepositoryFactory fileSystemGenericRepositoryFactory, IMapper mapper)
            : this(fileSystemGenericRepositoryFactory)
        {
            Mapper = mapper;
        }

        public BaseApplicationService(IFileSystemGenericRepositoryFactory fileSystemGenericRepositoryFactory)
        {
            if (fileSystemGenericRepositoryFactory == null) throw new ArgumentNullException("fileSystemGenericRepositoryFactory");
            _fileSystemGenericRepositoryFactory = fileSystemGenericRepositoryFactory;
        }

        public BaseApplicationService()
        {

        }

        public IFileSystemGenericRepositoryFactory FileSytemGenericRepositoryFactory
        {
            get
            {
                return _fileSystemGenericRepositoryFactory;
            }
        }

        public Expression<Func<TDestination, Object>>[] GetMappedIncludes<TSource, TDestination>(Expression<Func<TSource, Object>>[] selectors)
        {
            if (selectors == null)
                return new Expression<Func<TDestination, Object>>[] { };

            List<Expression<Func<TDestination, Object>>> returnList = new List<Expression<Func<TDestination, Object>>>();

            foreach (var selector in selectors)
            {
                returnList.Add(Mapper.Map<Expression<Func<TDestination, Object>>>(selector));
            }

            return returnList.ToArray();
        }

        public Expression<Func<TDestination, TProperty>> GetMappedSelector<TSource, TDestination, TProperty>(Expression<Func<TSource, TProperty>> selector)
        {
            return Mapper.Map<Expression<Func<TDestination, TProperty>>>(selector);
        }

        public Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>> GetMappedOrderBy<TSource, TDestination>(Expression<Func<IQueryable<TSource>, IOrderedQueryable<TSource>>> orderBy)
        {
            //return LamdaHelper.GetMappedOrderBy<TSource, TDestination>(Mapper, orderBy);
            if (orderBy == null)
                return null;

            //var typeMappings = new Dictionary<Type, Type>();

            //List<Type> sourceArguments = orderBy.GetType().GetGenericArguments()[0].GetGenericArguments().ToList();
            //List<Type> destArguments = typeof(Expression<Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>>>).GetGenericArguments()[0].GetGenericArguments().ToList();

            //DoAddTypeMappings(typeMappings, Mapper.ConfigurationProvider, sourceArguments, destArguments);

            return Mapper.Map<Expression<Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>>>>(orderBy).Compile();
        }

        public void DoAddTypeMappings( Dictionary<Type, Type> typeMappings, IConfigurationProvider configurationProvider, List<Type> sourceArguments, List<Type> destArguments)
        {

            for (int i = 0; i < sourceArguments.Count; i++)
            {
                if (!typeMappings.ContainsKey(sourceArguments[i]) && sourceArguments[i] != destArguments[i])
                    AddTypeMapping(typeMappings, configurationProvider, sourceArguments[i], destArguments[i]);
            }
        }

        public Dictionary<Type, Type> AddTypeMapping( Dictionary<Type, Type> typeMappings, IConfigurationProvider configurationProvider, Type sourceType, Type destType)
        {

            if (sourceType.GetTypeInfo().IsGenericType && sourceType.GetGenericTypeDefinition() == typeof(Expression<>))
            {
                sourceType = sourceType.GetGenericArguments()[0];
                destType = destType.GetGenericArguments()[0];
            }

            if (!typeMappings.ContainsKey(sourceType) && sourceType != destType)
            {
                typeMappings.Add(sourceType, destType);
                if (typeof(Delegate).IsAssignableFrom(sourceType))
                {
                    var r = true;
                }
                else
                {
                    AddUnderlyingTypes(typeMappings, configurationProvider, sourceType, destType);
                    FindChildPropertyTypeMaps(typeMappings, configurationProvider, sourceType, destType);
                }
            }

            return typeMappings;
        }

        private void FindChildPropertyTypeMaps( Dictionary<Type, Type> typeMappings, IConfigurationProvider ConfigurationProvider, Type source, Type dest)
        {
            //The destination becomes the source because to map a source expression to a destination expression,
            //we need the expressions used to create the source from the destination
            var typeMap = ConfigurationProvider.ResolveTypeMap(sourceType: dest, destinationType: source);

            if (typeMap == null)
                return;

            FindMaps(typeMap.GetPropertyMaps().ToList());
            void FindMaps(List<PropertyMap> maps)
            {
                foreach (PropertyMap pm in maps)
                {
                    if (pm.SourceMember == null)
                        continue;

                    AddChildMappings
                    (
                        GetMemberType(GetFieldOrProperty(source, pm.DestinationProperty.Name)),
                        GetMemberType(pm.SourceType) //Instead of pm.SourceMember
                    );
                    void AddChildMappings(Type sourcePropertyType, Type destPropertyType)
                    {
                        if (IsLiteralType(sourcePropertyType) || IsLiteralType(destPropertyType))
                            return;

                        AddTypeMapping(typeMappings, ConfigurationProvider, sourcePropertyType, destPropertyType);
                    }
                }
            }
        }

        public Type GetMemberType( MemberInfo memberInfo)
            => ReflectionHelper.GetMemberType(memberInfo);

        public MemberInfo GetFieldOrProperty( Type type, string name)
            => PrimitiveHelper.GetFieldOrProperty(type, name);


        public bool IsLiteralType( Type type)
        {
            if (PrimitiveHelper.IsNullableType(type))
                type = Nullable.GetUnderlyingType(type);

            return LiteralTypes.Contains(type);
        }

        private static HashSet<Type> LiteralTypes => new HashSet<Type>(_literalTypes);

        private static Type[] _literalTypes => new Type[] {
                typeof(bool),
                typeof(DateTime),
                typeof(TimeSpan),
                typeof(Guid),
                typeof(decimal),
                typeof(byte),
                typeof(short),
                typeof(int),
                typeof(long),
                typeof(float),
                typeof(double),
                typeof(char),
                typeof(sbyte),
                typeof(ushort),
                typeof(uint),
                typeof(ulong),
                typeof(string)
            };

        public void AddUnderlyingTypes( Dictionary<Type, Type> typeMappings, IConfigurationProvider configurationProvider, Type sourceType, Type destType)
        {
            DoAddTypeMappings
            (
                typeMappings,
                configurationProvider,
                !HasUnderlyingType(sourceType) ? new List<Type>() : ElementTypeHelper.GetElementTypes(sourceType).ToList(),
                !HasUnderlyingType(destType) ? new List<Type>() : ElementTypeHelper.GetElementTypes(destType).ToList()
            );
        }

        private bool HasUnderlyingType( Type type)
        {
            return (IsGenericType(type) && typeof(System.Collections.IEnumerable).IsAssignableFrom(type)) || type.IsArray;
        }

        public bool IsGenericType( Type type) => type.GetTypeInfo().IsGenericType;
    }
}
