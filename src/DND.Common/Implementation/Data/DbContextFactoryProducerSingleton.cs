using DND.Common.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DND.Common.Implementation.Data
{
    public class DbContextFactoryProducerSingleton : IDbContextFactoryProducerSingleton
    {
        private readonly Dictionary<Type, List<Type>> _entityTypeToFactoryMapper = new Dictionary<Type, List<Type>>();
        private readonly Dictionary<Type, IDbContextAbstractFactory> _dbContextAbstractFactoriesContainer = new Dictionary<Type, IDbContextAbstractFactory>(); 
        public DbContextFactoryProducerSingleton(IEnumerable<IDbContextAbstractFactory> dbContextFactories)
        {
            foreach (var dbContextFactory in dbContextFactories)
            {
               var dbContextAbstractFactoryInterfaces = dbContextFactory.GetType().GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDbContextFactory<>));
                foreach (var dbContextAbstractFactoryInterface in dbContextAbstractFactoryInterfaces)
                {
                   var dbContextType = dbContextAbstractFactoryInterface.GetGenericArguments().First();
                    _dbContextAbstractFactoriesContainer.Add(dbContextType, dbContextFactory);

                    var properties = dbContextType.GetProperties();

                    foreach (var property in properties)
                    {
                        var setType = property.PropertyType;

                        var isDbSet = setType.IsGenericType && (typeof(IDbSet<>).IsAssignableFrom(setType.GetGenericTypeDefinition()) || setType.GetInterface(typeof(IDbSet<>).FullName) != null);
                        isDbSet = isDbSet || setType.IsGenericType && (typeof(System.Data.Entity.DbSet<>).IsAssignableFrom(setType.GetGenericTypeDefinition()));
                        var isDbSetEFCore = setType.IsGenericType && (typeof(Microsoft.EntityFrameworkCore.DbSet<>).IsAssignableFrom(setType.GetGenericTypeDefinition()));

                        if (isDbSet || isDbSetEFCore)
                        {
                            var entityType = property.PropertyType.GetGenericArguments().First();
                            if(!_entityTypeToFactoryMapper.ContainsKey(entityType))
                            {
                                _entityTypeToFactoryMapper.Add(entityType, new List<Type>());
                            }
                            _entityTypeToFactoryMapper[entityType].Add(dbContextType);
                        }
                    }
                }
            }
        }

        public IDbContextAbstractFactory GetAbstractFactory<TIDbContext>() where TIDbContext : IBaseDbContext
        {
            if (_dbContextAbstractFactoriesContainer.ContainsKey(typeof(TIDbContext)))
            {
                return _dbContextAbstractFactoriesContainer[typeof(TIDbContext)];
            }
            else
            {
                throw new Exception("DbContext Type Unknown");
            }
        }


        public IDbContextAbstractFactory GetFactory(Type type)
        {
            if (_dbContextAbstractFactoriesContainer.ContainsKey(type))
            {
                return _dbContextAbstractFactoriesContainer[type];
            }
            else
            {
                throw new Exception("DbContext Type Unknown");
            }
        }

        public IDbContextAbstractFactory GetFactoryByEntityType(Type entityType)
        {
            if (_entityTypeToFactoryMapper.ContainsKey(entityType))
            {
                return GetFactory(_entityTypeToFactoryMapper[entityType].First());
            }
            else
            {
                throw new Exception("Entity Type Unknown");
            }
        }

        public IDbContextFactory<TIDbContext> GetFactory<TIDbContext>() where TIDbContext : IBaseDbContext
        {
            if(_dbContextAbstractFactoriesContainer.ContainsKey(typeof(TIDbContext)))
            {
                return (IDbContextFactory<TIDbContext>)_dbContextAbstractFactoriesContainer[typeof(TIDbContext)];
            }
            else
            {
                throw new Exception("DbContext Type Unknown");
            }
        }
    }
}
