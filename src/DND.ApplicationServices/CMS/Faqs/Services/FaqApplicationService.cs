using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Interfaces.Persistance;
using DND.Domain.CMS.Faqs;
using DND.Domain.CMS.Faqs.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;

namespace DND.ApplicationServices.CMS.Faqs.Services
{
    public class FaqApplicationService : BaseEntityApplicationService<IBaseDbContext, Faq, FaqDto, FaqDto, FaqDto, FaqDeleteDto, IFaqDomainService>, IFaqApplicationService
    {
        public FaqApplicationService(IFaqDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
    }
}
