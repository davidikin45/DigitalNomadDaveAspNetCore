using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using Solution.Base.Interfaces.Models;
using Solution.Base.Interfaces.Repository;
using Solution.Base.Interfaces.UnitOfWork;
using Solution.Base.Interfaces.Persistance;

using Solution.Base.Implementation.Repository.EntityFramework;
using System.Threading;

namespace Solution.Base.Implementation.UnitOfWork
{
    public class BaseUnitOfWorkScope : IBaseUnitOfWorkScope
    {
        ConcurrentDictionary<Type, object> _repositories = new ConcurrentDictionary<Type, object>();
        ConcurrentDictionary<Type, object> _readOnlyRepositories = new ConcurrentDictionary<Type, object>();
        private readonly IBaseAmbientDbContextLocator _contextLocator;
        private readonly IBaseRepositoryFactory _repositoryFactory;
        protected BaseDbContextCollection _dbContexts;
        private readonly CancellationToken _cancellationToken;

        public virtual IBaseDbContextCollection DbContexts { get { return _dbContexts; } }

        public BaseUnitOfWorkScope(IBaseAmbientDbContextLocator contextLocator, IBaseRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (contextLocator == null) throw new ArgumentNullException("contextLocator");
            if (repositoryFactory == null) throw new ArgumentNullException("repositoryFactory");
            _contextLocator = contextLocator;
            _repositoryFactory = repositoryFactory;
            _cancellationToken = cancellationToken;
        }

        public IBaseRepository<TEntity> Repository<TContext, TEntity>()
             where TContext : IBaseDbContext
            where TEntity : class, IBaseEntity, IBaseEntityAuditable, new()
        {
            return (IBaseRepository<TEntity>)_repositories.GetOrAdd(
                typeof(TEntity),
                t => _repositoryFactory.Get<TEntity>(DbContexts.Get<TContext>(), _cancellationToken)
            );
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
