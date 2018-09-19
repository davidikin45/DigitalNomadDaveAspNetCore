using DND.Common.Infrastructure.Interfaces.Data;
using DND.Common.Infrastructure.Interfaces.Data.Repository.GenericEF;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using System;
using System.Data;

namespace Toptal.Common.Data.UnitOfWork
{
    public class UnitOfWorkReadOnlyScope : UnitOfWorkScopeBase, IDisposable, IUnitOfWorkReadOnlyScope
    {
        private UnitOfWorkTransactionScope _internalScope;

        public override IDbContextCollection DbContexts { get { return _internalScope.DbContexts; } }

        public UnitOfWorkReadOnlyScope(IDbContextFactoryProducerSingleton dbContextFactory = null, IAmbientDbContextLocator contextLocator = null, IGenericRepositoryFactory repositoryFactory = null)
            : this(joiningOption: UnitOfWorkScopeOption.JoinExisting, isolationLevel: null, dbContextFactory: dbContextFactory)
        {}

        public UnitOfWorkReadOnlyScope(IsolationLevel isolationLevel, IDbContextFactoryProducerSingleton dbContextFactory = null, IAmbientDbContextLocator contextLocator = null, IGenericRepositoryFactory repositoryFactory = null)
            : this(joiningOption: UnitOfWorkScopeOption.ForceCreateNew, isolationLevel: isolationLevel, dbContextFactory: dbContextFactory)
        { }

        public UnitOfWorkReadOnlyScope(UnitOfWorkScopeOption joiningOption, IsolationLevel? isolationLevel, IDbContextFactoryProducerSingleton dbContextFactory = null, IAmbientDbContextLocator contextLocator = null, IGenericRepositoryFactory repositoryFactory = null)
           : base(contextLocator: contextLocator, repositoryFactory: repositoryFactory)
        {
            _internalScope = new UnitOfWorkTransactionScope(joiningOption: joiningOption, readOnly: true, isolationLevel: isolationLevel, dbContextFactory: dbContextFactory, contextLocator: contextLocator, repositoryFactory: repositoryFactory );
        }

        public void Dispose()
        {
            _internalScope.Dispose();
        }
    }
}