using DND.Common.Data.UnitOfWork;
using DND.Common.Infrastructure.Interfaces.Data.Repository.GenericEF;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Toptal.Common.Data.UnitOfWork
{
    public class UnitOfWorkScopeBase : IUnitOfWorkScope
    {
        protected ConcurrentDictionary<Type, object> _repositories = new ConcurrentDictionary<Type, object>();
        ConcurrentDictionary<Type, object> _readOnlyRepositories = new ConcurrentDictionary<Type, object>();
        private readonly IAmbientDbContextLocator _contextLocator;
        protected readonly IGenericRepositoryFactory _repositoryFactory;
        protected DbContextCollection _dbContexts;
        protected readonly CancellationToken _cancellationToken;

        public virtual IDbContextCollection DbContexts { get { return _dbContexts; } }

        public UnitOfWorkScopeBase(IAmbientDbContextLocator contextLocator, IGenericRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (contextLocator == null) throw new ArgumentNullException("contextLocator");
            if (repositoryFactory == null) throw new ArgumentNullException("repositoryFactory");
            _contextLocator = contextLocator;
            _repositoryFactory = repositoryFactory;
            _cancellationToken = cancellationToken;
        }

        public IGenericEFReadOnlyRepository<TEntity> ReadOnlyRepository<TContext, TEntity>()
             where TContext : DbContext
            where TEntity : class
        {
            return (IGenericEFReadOnlyRepository<TEntity>)_readOnlyRepositories.GetOrAdd(
                typeof(TEntity),
                t => _repositoryFactory.GetReadOnly<TEntity>(DbContexts.Get<TContext>())
            );
        }

    }
}
