using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.CMS.Projects.Dtos;

namespace DND.Interfaces.CMS.ApplicationServices
{
    public interface IProjectApplicationService : IBaseEntityApplicationService<ProjectDto, ProjectDto, ProjectDto, ProjectDeleteDto>
    {
        
    }
}
