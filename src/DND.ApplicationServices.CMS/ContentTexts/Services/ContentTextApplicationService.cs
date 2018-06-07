using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.CMS.ContentTexts;
using DND.Domain.CMS.ContentTexts.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Interfaces.CMS.DomainServices;

namespace DND.ApplicationServices.CMS.ContentTexts.Services
{
    public class ContentTextApplicationService : BaseEntityApplicationService<ContentText, ContentTextDto, ContentTextDto, ContentTextDto, ContentTextDeleteDto, IContentTextDomainService>, IContentTextApplicationService
    {
        public ContentTextApplicationService(IContentTextDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
    }
}
