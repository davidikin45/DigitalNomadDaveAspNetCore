using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Models;
using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.Persistance;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.CMS.ContentTexts;

namespace DND.DomainServices.CMS.ContentTexts.Services
{
    public class ContentTextDomainService : BaseEntityDomainService<IBaseDbContext, ContentText>, IContentTextDomainService
    {
        public ContentTextDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
    }
}
