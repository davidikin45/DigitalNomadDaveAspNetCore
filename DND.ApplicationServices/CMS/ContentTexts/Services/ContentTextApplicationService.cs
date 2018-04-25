using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Interfaces.Persistance;
using DND.Domain.CMS.ContentTexts;
using DND.Domain.CMS.ContentTexts.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;

namespace DND.ApplicationServices.CMS.ContentTexts.Services
{
    public class ContentTextApplicationService : BaseEntityApplicationService<IBaseDbContext, ContentText, ContentTextDto, ContentTextDto, ContentTextDto, ContentTextDeleteDto, IContentTextDomainService>, IContentTextApplicationService
    {
        public ContentTextApplicationService(IContentTextDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
    }
}
