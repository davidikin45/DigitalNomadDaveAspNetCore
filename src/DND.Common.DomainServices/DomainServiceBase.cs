using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Common.Infrastructure.Interfaces.DomainServices;
using System;

namespace DND.Common.DomainServices
{
    public abstract class DomainServiceBase : IDomainService
    {
        private readonly IUnitOfWorkScopeFactory _baseUnitOfWorkScopeFactory;

        public DomainServiceBase(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        {
            if (baseUnitOfWorkScopeFactory == null) throw new ArgumentNullException("baseUnitOfWorkScopeFactory");
            _baseUnitOfWorkScopeFactory = baseUnitOfWorkScopeFactory;
        }

        public IUnitOfWorkScopeFactory UnitOfWorkFactory
        {
            get
            {
                return _baseUnitOfWorkScopeFactory;
            }
        }
    }
}
