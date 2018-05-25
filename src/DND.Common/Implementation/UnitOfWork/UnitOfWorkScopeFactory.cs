/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */
using System;
using System.Data;

using DND.Common.Interfaces.UnitOfWork;
using DND.Common.Interfaces.Repository;
using DND.Common.Interfaces.Persistance;
using System.Threading;

namespace DND.Common.Implementation.UnitOfWork
{
    public class UnitOfWorkScopeFactory : IUnitOfWorkScopeFactory
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IAmbientDbContextLocator _contextLocator;
        private readonly IGenericRepositoryFactory _repositoryFactory;

        public UnitOfWorkScopeFactory(IDbContextFactory dbContextFactory = null, IAmbientDbContextLocator contextLocator = null, IGenericRepositoryFactory repositoryFactory = null)
        {
            _dbContextFactory = dbContextFactory;
            _contextLocator = contextLocator;
            _repositoryFactory = repositoryFactory;
        }

        public IUnitOfWorkTransactionScope Create(BaseUnitOfWorkScopeOption joiningOption = BaseUnitOfWorkScopeOption.JoinExisting, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new UnitOfWorkTransactionScope(
                joiningOption: joiningOption, 
                readOnly: false, 
                isolationLevel: null, 
                dbContextFactory: _dbContextFactory,
                contextLocator:_contextLocator,
                repositoryFactory:_repositoryFactory,
                cancellationToken: cancellationToken);
        }

        public IUnitOfWorkReadOnlyScope CreateReadOnly(BaseUnitOfWorkScopeOption joiningOption = BaseUnitOfWorkScopeOption.JoinExisting, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new UnitOfWorkReadOnlyScope(
                joiningOption: joiningOption, 
                isolationLevel: null,
                dbContextFactory: _dbContextFactory,
                contextLocator: _contextLocator,
                repositoryFactory: _repositoryFactory,
                cancellationToken: cancellationToken);
        }

        public IUnitOfWorkTransactionScope CreateWithTransaction(IsolationLevel isolationLevel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return new UnitOfWorkTransactionScope(
                joiningOption: BaseUnitOfWorkScopeOption.ForceCreateNew, 
                readOnly: false, 
                isolationLevel: isolationLevel,
                dbContextFactory: _dbContextFactory,
                contextLocator: _contextLocator,
                repositoryFactory: _repositoryFactory,
                cancellationToken: cancellationToken);
        }

        public IUnitOfWorkReadOnlyScope CreateReadOnlyWithTransaction(IsolationLevel isolationLevel, CancellationToken cancellationToken = default(CancellationToken))
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