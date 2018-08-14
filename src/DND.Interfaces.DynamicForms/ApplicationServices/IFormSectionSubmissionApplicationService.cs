using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.DynamicForms.FormSectionSubmissions.Dtos;

namespace DND.Interfaces.DynamicForms.ApplicationServices
{
    public interface IFormSectionSubmissionApplicationService : IBaseEntityApplicationService<FormSectionSubmissionDto, FormSectionSubmissionDto, FormSectionSubmissionDto, FormSectionSubmissionDeleteDto>
    {

    }
}
