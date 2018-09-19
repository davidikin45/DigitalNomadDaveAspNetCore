using DND.Common.DomainServices;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Data;
using DND.Domain.CMS.ContentTexts;
using DND.Interfaces.CMS.DomainServices;

namespace DND.DomainServices.CMS.ContentTexts.Services
{
    public class ContentTextDomainService : DomainServiceEntityBase<ApplicationContext, ContentText>, IContentTextDomainService
    {
        public ContentTextDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

    }
}
