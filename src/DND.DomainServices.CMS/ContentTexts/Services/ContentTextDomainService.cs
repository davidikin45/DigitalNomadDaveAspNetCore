using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.CMS.ContentTexts;
using DND.Interfaces.CMS.Data;
using DND.Interfaces.CMS.DomainServices;

namespace DND.DomainServices.CMS.ContentTexts.Services
{
    public class ContentTextDomainService : BaseEntityDomainService<ICMSDbContext, ContentText>, IContentTextDomainService
    {
        public ContentTextDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

    }
}
