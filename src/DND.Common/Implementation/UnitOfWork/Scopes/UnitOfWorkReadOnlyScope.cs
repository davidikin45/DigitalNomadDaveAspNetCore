﻿/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.

using DND.Core.Enums; */
using System.Data;

using DND.Common.Interfaces.UnitOfWork;
using DND.Common.Interfaces.Repository;
using DND.Common.Interfaces.Persistance;
using System.Threading;

namespace DND.Common.Implementation.UnitOfWork
{
    public class UnitOfWorkReadOnlyScope : BaseUnitOfWorkScope, IUnitOfWorkReadOnlyScope
    {
        private UnitOfWorkTransactionScope _internalScope;

        public override IDbContextCollection DbContexts { get { return _internalScope.DbContexts; } }

        public UnitOfWorkReadOnlyScope(IDbContextFactory dbContextFactory = null, IAmbientDbContextLocator contextLocator = null, IGenericRepositoryFactory repositoryFactory = null, CancellationToken cancellationToken = default(CancellationToken))
            : this(joiningOption: BaseUnitOfWorkScopeOption.JoinExisting, isolationLevel: null, dbContextFactory: dbContextFactory, cancellationToken: cancellationToken)
        {}

        public UnitOfWorkReadOnlyScope(IsolationLevel isolationLevel, IDbContextFactory dbContextFactory = null, IAmbientDbContextLocator contextLocator = null, IGenericRepositoryFactory repositoryFactory = null, CancellationToken cancellationToken = default(CancellationToken))
            : this(joiningOption: BaseUnitOfWorkScopeOption.ForceCreateNew, isolationLevel: isolationLevel, dbContextFactory: dbContextFactory, cancellationToken: cancellationToken)
        { }

        public UnitOfWorkReadOnlyScope(BaseUnitOfWorkScopeOption joiningOption, IsolationLevel? isolationLevel, IDbContextFactory dbContextFactory = null, IAmbientDbContextLocator contextLocator = null, IGenericRepositoryFactory repositoryFactory = null, CancellationToken cancellationToken = default(CancellationToken))
           : base(contextLocator: contextLocator, repositoryFactory: repositoryFactory, cancellationToken: cancellationToken)
        {
            _internalScope = new UnitOfWorkTransactionScope(joiningOption: joiningOption, readOnly: true, isolationLevel: isolationLevel, dbContextFactory: dbContextFactory, contextLocator: contextLocator, repositoryFactory: repositoryFactory );
        }

        public new void Dispose()
        {
            _internalScope.Dispose();
        }
    }
}