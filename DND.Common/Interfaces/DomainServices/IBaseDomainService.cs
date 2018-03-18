
using DND.Common.Interfaces.UnitOfWork;

namespace DND.Common.Interfaces.DomainServices
{
    public interface IBaseDomainService
    {
        IBaseUnitOfWorkScopeFactory UnitOfWorkFactory { get; }
    }
}
