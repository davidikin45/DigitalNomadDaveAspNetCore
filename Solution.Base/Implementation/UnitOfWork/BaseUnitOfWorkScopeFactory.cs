/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */
using System;
using System.Data;

using Solution.Base.Interfaces.UnitOfWork;
using Solution.Base.Interfaces.Repository;
using Solution.Base.Interfaces.Persistance;
using System.Threading;

namespace Solution.Base.Implementation.UnitOfWork
{
    public class BaseUnitOfWorkScopeFactory : IBaseUnitOfWorkScopeFactory
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IBaseAmbientDbContextLocator _contextLocator;
        private readonly IBaseRepositoryFactory _repositoryFactory;

        public BaseUnitOfWorkScopeFactory(IDbContextFactory dbContextFactory = null, IBaseAmbientDbContextLocator contextLocator = null, IBaseRepositoryFactory repositoryFactory = null)
        {
            _dbContextFactory = dbContextFactory;
            _contextLocator = contextLocator;
            _repositoryFactory = repositoryFactory;
        }

        public IBaseUnitOfWorkTransactionScope Create(BaseUnitOfWorkScopeOption joiningOption = BaseUnitOfWorkScopeOption.JoinExisting, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new BaseUnitOfWorkTransactionScope(
                joiningOption: joiningOption, 
                readOnly: false, 
                isolationLevel: null, 
                dbContextFactory: _dbContextFactory,
                contextLocator:_contextLocator,
                repositoryFactory:_repositoryFactory,
                cancellationToken: cancellationToken);
        }

        public IBaseUnitOfWorkReadOnlyScope CreateReadOnly(BaseUnitOfWorkScopeOption joiningOption = BaseUnitOfWorkScopeOption.JoinExisting, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new UnitOfWorkReadOnlyScope(
                joiningOption: joiningOption, 
                isolationLevel: null,
                dbContextFactory: _dbContextFactory,
                contextLocator: _contextLocator,
                repositoryFactory: _repositoryFactory,
                cancellationToken: cancellationToken);
        }

        public IBaseUnitOfWorkTransactionScope CreateWithTransaction(IsolationLevel isolationLevel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new BaseUnitOfWorkTransactionScope(
                joiningOption: BaseUnitOfWorkScopeOption.ForceCreateNew, 
                readOnly: false, 
                isolationLevel: isolationLevel,
                dbContextFactory: _dbContextFactory,
                contextLocator: _contextLocator,
                repositoryFactory: _repositoryFactory,
                cancellationToken: cancellationToken);
        }

        public IBaseUnitOfWorkReadOnlyScope CreateReadOnlyWithTransaction(IsolationLevel isolationLevel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new UnitOfWorkReadOnlyScope(
                joiningOption: BaseUnitOfWorkScopeOption.ForceCreateNew, 
                isolationLevel: isolationLevel,
                dbContextFactory: _dbContextFactory,
                contextLocator: _contextLocator,
                repositoryFactory: _repositoryFactory,
                cancellationToken: cancellationToken);
        }

        public IDisposable SuppressAmbientContext()
        {
            return new AmbientContextSuppressor();
        }
    }
}