
using DND.Common.Interfaces.UnitOfWork;

namespace DND.Common.Interfaces.DomainServices
{
    public interface IBaseDomainService
    {
        IUnitOfWorkScopeFactory UnitOfWorkFactory { get; }
    }
}
