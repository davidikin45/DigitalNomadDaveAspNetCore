using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.CMS.Projects;
using DND.Common.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DND.DomainServices.CMS.Projects.Services
{
    public class ProjectDomainService : BaseEntityDomainService<IApplicationDbContext, Project>, IProjectDomainService
    {
        public ProjectDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {
        }

        public async override Task<IEnumerable<ValidationResult>> DbDependantValidateAsync(Project entity, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();

            return errors;
        }
    }
}