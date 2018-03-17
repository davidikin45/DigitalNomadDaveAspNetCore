using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Models;
using Solution.Base.Implementation.DomainServices;
using Solution.Base.Interfaces.Persistance;
using Solution.Base.Interfaces.UnitOfWork;

namespace DND.ApplicationServices
{
    public class ContentTextDomainService : BaseEntityDomainService<IBaseDbContext, ContentText>, IContentTextDomainService
    {
        public ContentTextDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
    }
}
