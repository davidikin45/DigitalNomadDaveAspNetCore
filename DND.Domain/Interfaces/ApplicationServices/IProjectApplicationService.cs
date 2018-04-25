using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.CMS.Projects.Dtos;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface IProjectApplicationService : IBaseEntityApplicationService<ProjectDto, ProjectDto, ProjectDto, ProjectDeleteDto>
    {
        
    }
}
