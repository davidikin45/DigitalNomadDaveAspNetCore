/* 
 * Copyright (C) 2014 Mehdi El Gueddari
 * http://mehdi.me
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 */
using DND.Common.Infrastructure.Interfaces.Data;
using DND.Common.Infrastructure.Interfaces.Data.Repository.GenericEF;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Common.Infrastructure.Interfaces.DomainEvents;
using System;
using System.Data;
using Toptal.Common.Data.UnitOfWork;

namespace DND.Common.Data.UnitOfWork
{
    public class UnitOfWorkScopeFactory : IUnitOfWorkScopeFactory
    {
        private readonly IDbContextFactoryProducerSingleton _dbContextFactory;
        private readonly IAmbientDbContextLocator _contextLocator;
        private readonly IGenericRepositoryFactory _repositoryFactory;
        private readonly IDomainEvents _domainEvents;

        public UnitOfWorkScopeFactory(IDbContextFactoryProducerSingleton dbContextFactory = null, IAmbientDbContextLocator contextLocator = null, IGenericRepositoryFactory repositoryFactory = null, IDomainEvents domainEvents = null)
        {
            _dbContextFactory = dbContextFactory;
            _contextLocator = contextLocator;
            _repositoryFactory = repositoryFactory;
            _domainEvents = domainEvents;
        }

        public IUnitOfWorkTransactionScope Create(UnitOfWorkScopeOption joiningOption = UnitOfWorkScopeOption.JoinExisting)
        {
            return new UnitOfWorkTransactionScope(
                joiningOption: joiningOption, 
                readOnly: false, 
                isolationLevel: null, 
                dbContextFactory: _dbContextFactory,
                contextLocator:_contextLocator,
                repositoryFactory:_repositoryFactory);
        }

        public IUnitOfWorkReadOnlyScope CreateReadOnly(UnitOfWorkScopeOption joiningOption = UnitOfWorkScopeOption.JoinExisting)
        {
            return new UnitOfWorkReadOnlyScope(
                joiningOption: joiningOption, 
                isolationLevel: null,
                dbContextFactory: _dbContextFactory,
                contextLocator: _contextLocator,
                repositoryFactory: _repositoryFactory);
        }

        public IUnitOfWorkTransactionScope CreateWithTransaction(IsolationLevel isolationLevel)
        {
            return new UnitOfWorkTransactionScope(
                joiningOption: UnitOfWorkScopeOption.ForceCreateNew, 
                readOnly: false, 
                isolationLevel: isolationLevel,
                dbContextFactory: _dbContextFactory,
                contextLocator: _contextLocator,
                repositoryFactory: _repositoryFactory);
        }

        public IUnitOfWorkReadOnlyScope CreateReadOnlyWithTransaction(IsolationLevel isolationLevel)
        {
            return new UnitOfWorkReadOnlyScope(
                joiningOption: UnitOfWorkScopeOption.ForceCreateNew, 
                isolationLevel: isolationLevel,
                dbContextFactory: _dbContextFactory,
                contextLocator: _contextLocator,
                repositoryFactory: _repositoryFactory);
        }

        public IDisposable SuppressAmbientContext()
        {
            return new AmbientContextSuppressor();
        }
    }
}