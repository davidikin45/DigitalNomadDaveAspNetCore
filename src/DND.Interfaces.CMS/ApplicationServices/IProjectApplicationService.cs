using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.CMS.Projects.Dtos;

namespace DND.Interfaces.CMS.ApplicationServices
{
    public interface IProjectApplicationService : IApplicationServiceEntity<ProjectDto, ProjectDto, ProjectDto, ProjectDeleteDto>
    {
        
    }
}
