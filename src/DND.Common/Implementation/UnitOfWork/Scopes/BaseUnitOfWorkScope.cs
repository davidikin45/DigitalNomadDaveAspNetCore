using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Repository;
using DND.Common.Interfaces.UnitOfWork;
using DND.Common.Interfaces.Persistance;

using DND.Common.Implementation.Repository.EntityFramework;
using System.Threading;

namespace DND.Common.Implementation.UnitOfWork
{
    public class BaseUnitOfWorkScope : IBaseUnitOfWorkScope
    {
        protected ConcurrentDictionary<Type, object> _repositories = new ConcurrentDictionary<Type, object>();
        ConcurrentDictionary<Type, object> _readOnlyRepositories = new ConcurrentDictionary<Type, object>();
        private readonly IBaseAmbientDbContextLocator _contextLocator;
        protected readonly IBaseRepositoryFactory _repositoryFactory;
        protected BaseDbContextCollection _dbContexts;
        protected readonly CancellationToken _cancellationToken;

        public virtual IBaseDbContextCollection DbContexts { get { return _dbContexts; } }

        public BaseUnitOfWorkScope(IBaseAmbientDbContextLocator contextLocator, IBaseRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (contextLocator == null) throw new ArgumentNullException("contextLocator");
            if (repositoryFactory == null) throw new ArgumentNullException("repositoryFactory");
            _contextLocator = contextLocator;
            _repositoryFactory = repositoryFactory;
            _cancellationToken = cancellationToken;
        }

        public IBaseReadOnlyRepository<TEntity> ReadOnlyRepository<TContext, TEntity>()
             where TContext : IBaseDbContext
            where TEntity : class, IBaseEntity, IBaseEntityAuditable, new()
        {
            return (IBaseReadOnlyRepository<TEntity>)_readOnlyRepositories.GetOrAdd(
                typeof(TEntity),
                t => _repositoryFactory.GetReadOnly<TEntity>(DbContexts.Get<TContext>(), _cancellationToken)
            );
        }

    }
}
