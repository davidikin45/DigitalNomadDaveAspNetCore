using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.DynamicForms.Sections.Dtos;

namespace DND.Interfaces.DynamicForms.ApplicationServices
{
    public interface ISectionApplicationService : IApplicationServiceEntity<SectionDto, SectionDto, SectionDto, SectionDeleteDto>
    {

    }
}
