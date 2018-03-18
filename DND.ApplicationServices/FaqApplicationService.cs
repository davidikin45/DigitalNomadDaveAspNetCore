using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Models;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Interfaces.Persistance;

namespace DND.ApplicationServices
{
    public class FaqApplicationService : BaseEntityApplicationService<IBaseDbContext, Faq, FaqDTO, IFaqDomainService>, IFaqApplicationService
    {
        public FaqApplicationService(IFaqDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
    }
}
