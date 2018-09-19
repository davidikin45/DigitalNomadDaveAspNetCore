using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.DynamicForms.FormSectionSubmissions.Dtos;

namespace DND.Interfaces.DynamicForms.ApplicationServices
{
    public interface IFormSectionSubmissionApplicationService : IApplicationServiceEntity<FormSectionSubmissionDto, FormSectionSubmissionDto, FormSectionSubmissionDto, FormSectionSubmissionDeleteDto>
    {

    }
}
