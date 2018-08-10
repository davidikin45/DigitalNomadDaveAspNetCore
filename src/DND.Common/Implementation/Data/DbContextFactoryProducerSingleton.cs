using DND.Common.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DND.Common.Implementation.Data
{
    public class DbContextFactoryProducerSingleton : IDbContextFactoryProducerSingleton
    {
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
