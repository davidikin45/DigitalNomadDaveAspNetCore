using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Services;
using DND.Domain.Models;
using Solution.Base.Implementation.Services;
using Solution.Base.Interfaces.Persistance;
using Solution.Base.Interfaces.UnitOfWork;

namespace DND.Services
{
    public class FaqService : BaseEntityService<IBaseDbContext, Faq, FaqDTO>, IFaqService
    {
        public FaqService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory, IMapper mapper)
        : base(baseUnitOfWorkScopeFactory, mapper)
        {

        }
    }
}
