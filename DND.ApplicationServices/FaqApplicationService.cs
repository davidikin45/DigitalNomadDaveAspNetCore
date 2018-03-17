using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Models;
using Solution.Base.Implementation.ApplicationServices;
using Solution.Base.Interfaces.Persistance;

namespace DND.ApplicationServices
{
    public class FaqApplicationService : BaseEntityApplicationService<IBaseDbContext, Faq, FaqDTO>, IFaqApplicationService
    {
        public FaqApplicationService(IFaqDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }
    }
}
