using AutoMapper;
using DND.Domain.Interfaces.Services;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using Solution.Base.Implementation.Services;
using Solution.Base.Interfaces.UnitOfWork;

namespace DND.Services
{
    public class ProjectService : BaseEntityService<IApplicationDbContext, Project, ProjectDTO>, IProjectService
    {
        public ProjectService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory, IMapper mapper)
        : base(baseUnitOfWorkScopeFactory, mapper)
        {

        }
     
    }
}