using AutoMapper;
using DND.Domain.Interfaces.Services;
using DND.Domain.DTOs;
using DND.Domain.Models;
using Solution.Base.Implementation.Services;
using Solution.Base.Interfaces.Persistance;
using Solution.Base.Interfaces.UnitOfWork;

namespace DND.Services
{
    public class ContentTextService : BaseEntityService<IBaseDbContext, ContentText, ContentTextDTO>, IContentTextService
    {
        public ContentTextService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory, IMapper mapper)
        : base(baseUnitOfWorkScopeFactory, mapper)
        {

        }
    }
}
