using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.DynamicForms.FormSectionSubmissions.Dtos;

namespace DND.Interfaces.DynamicForms.DomainServices
{
    public interface IFormSectionSubmissionApplicationService : IBaseEntityApplicationService<FormSectionSubmissionDto, FormSectionSubmissionDto, FormSectionSubmissionDto, FormSectionSubmissionDeleteDto>
    {

    }
}
