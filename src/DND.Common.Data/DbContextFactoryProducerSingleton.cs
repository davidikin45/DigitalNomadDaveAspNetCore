using DND.Common.Infrastructure.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DND.Common.Data
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

                        var isDbSetEFCore = setType.IsGenericType && (typeof(DbSet<>).IsAssignableFrom(setType.GetGenericTypeDefinition()));

                        if (isDbSetEFCore)
                        {
                            var entityType = property.PropertyType.GetGenericArguments().First();
                            if (!_entityTypeToFactoryMapper.ContainsKey(entityType))
                            {
                                _entityTypeToFactoryMapper.Add(entityType, new List<Type>());
                            }
                            _entityTypeToFactoryMapper[entityType].Add(dbContextType);
                        }
                    }
                }
            }
        }

        public IDbContextAbstractFactory GetAbstractFactory<TDbContext>() where TDbContext : DbContext
        {
            if (_dbContextAbstractFactoriesContainer.ContainsKey(typeof(TDbContext)))
            {
                return _dbContextAbstractFactoriesContainer[typeof(TDbContext)];
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

        public IDbContextFactory<TDbContext> GetFactory<TDbContext>() where TDbContext : DbContext
        {
            if (_dbContextAbstractFactoriesContainer.ContainsKey(typeof(TDbContext)))
            {
                return (IDbContextFactory<TDbContext>)_dbContextAbstractFactoriesContainer[typeof(TDbContext)];
            }
            else
            {
                throw new Exception("DbContext Type Unknown");
            }
        }
    }
}
