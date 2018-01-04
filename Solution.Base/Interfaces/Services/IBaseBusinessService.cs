
using Solution.Base.Interfaces.UnitOfWork;

namespace Solution.Base.Interfaces.Services
{
    public interface IBaseBusinessService
    {
        IBaseUnitOfWorkScopeFactory UnitOfWorkFactory { get; }
    }
}
