using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Models;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Interfaces.Persistance;

namespace DND.ApplicationServices
{
    public class ContentTextApplicationService : BaseEntityApplicationService<IBaseDbContext, ContentText, ContentTextDTO, IContentTextDomainService>, IContentTextApplicationService
    {
        public ContentTextApplicationService(IContentTextDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
    }
}
