using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;

namespace DND.Common.Infrastructure.Interfaces.DomainServices
{
    public interface IDomainService
    {
        IUnitOfWorkScopeFactory UnitOfWorkFactory { get; }
    }
}
