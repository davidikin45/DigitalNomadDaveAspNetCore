using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.DynamicForms.FormSubmissions.Dtos;

namespace DND.Interfaces.DynamicForms.ApplicationServices
{
    public interface IFormSubmissionApplicationService : IApplicationServiceEntity<FormSubmissionDto, FormSubmissionDto, FormSubmissionDto, FormSubmissionDeleteDto>
    {

    }
}
