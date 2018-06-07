using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.CMS.Faqs;
using DND.Domain.CMS.Faqs.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Interfaces.CMS.DomainServices;

namespace DND.ApplicationServices.CMS.Faqs.Services
{
    public class FaqApplicationService : BaseEntityApplicationService<Faq, FaqDto, FaqDto, FaqDto, FaqDeleteDto, IFaqDomainService>, IFaqApplicationService
    {
        public FaqApplicationService(IFaqDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
    }
}
