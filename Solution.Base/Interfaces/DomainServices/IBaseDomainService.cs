
using Solution.Base.Interfaces.UnitOfWork;

namespace Solution.Base.Interfaces.DomainServices
{
    public interface IBaseDomainService
    {
        IBaseUnitOfWorkScopeFactory UnitOfWorkFactory { get; }
    }
}
