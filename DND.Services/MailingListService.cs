using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Services;
using DND.Domain.Models;
using Solution.Base.Implementation.Services;
using Solution.Base.Interfaces.Persistance;
using Solution.Base.Interfaces.UnitOfWork;

namespace DND.Services
{
    public class MailingListService : BaseEntityService<IBaseDbContext, MailingList, MailingListDTO>, IMailingListService
    {
        public MailingListService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory, IMapper mapper)
        : base(baseUnitOfWorkScopeFactory, mapper)
        {

        }
    }
}
