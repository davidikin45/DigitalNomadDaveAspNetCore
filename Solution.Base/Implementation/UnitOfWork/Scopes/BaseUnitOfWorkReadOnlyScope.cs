/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.

using DND.Core.Enums; */
using System.Data;

using Solution.Base.Interfaces.UnitOfWork;
using Solution.Base.Interfaces.Repository;
using Solution.Base.Interfaces.Persistance;
using System.Threading;

namespace Solution.Base.Implementation.UnitOfWork
{
    public class UnitOfWorkReadOnlyScope : BaseUnitOfWorkScope, IBaseUnitOfWorkReadOnlyScope
    {
        private BaseUnitOfWorkTransactionScope _internalScope;

        public override IBaseDbContextCollection DbContexts { get { return _internalScope.DbContexts; } }

        public UnitOfWorkReadOnlyScope(IDbContextFactory dbContextFactory = null, IBaseAmbientDbContextLocator contextLocator = null, IBaseRepositoryFactory repositoryFactory = null, CancellationToken cancellationToken = default(CancellationToken))
            : this(joiningOption: BaseUnitOfWorkScopeOption.JoinExisting, isolationLevel: null, dbContextFactory: dbContextFactory, cancellationToken: cancellationToken)
        {}

        public UnitOfWorkReadOnlyScope(IsolationLevel isolationLevel, IDbContextFactory dbContextFactory = null, IBaseAmbientDbContextLocator contextLocator = null, IBaseRepositoryFactory repositoryFactory = null, CancellationToken cancellationToken = default(CancellationToken))
            : this(joiningOption: BaseUnitOfWorkScopeOption.ForceCreateNew, isolationLevel: isolationLevel, dbContextFactory: dbContextFactory, cancellationToken: cancellationToken)
        { }

        public UnitOfWorkReadOnlyScope(BaseUnitOfWorkScopeOption joiningOption, IsolationLevel? isolationLevel, IDbContextFactory dbContextFactory = null, IBaseAmbientDbContextLocator contextLocator = null, IBaseRepositoryFactory repositoryFactory = null, CancellationToken cancellationToken = default(CancellationToken))
           : base(contextLocator: contextLocator, repositoryFactory: repositoryFactory, cancellationToken: cancellationToken)
        {
            _internalScope = new BaseUnitOfWorkTransactionScope(joiningOption: joiningOption, readOnly: true, isolationLevel: isolationLevel, dbContextFactory: dbContextFactory, contextLocator: contextLocator, repositoryFactory: repositoryFactory );
        }

        public void Dispose()
        {
            _internalScope.Dispose();
        }
    }
}