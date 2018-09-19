using DND.Common.Infrastructure.Interfaces.ApplicationServices;

namespace DND.Interfaces.DynamicForms.ApplicationServices
{
    public interface IDynamicFormsApplicationServices : IApplicationService
    {
        IFormApplicationService FormApplicationService { get; }
        IFormSubmissionApplicationService FormSubmissionApplicationService { get; }
        IFormSectionSubmissionApplicationService FormSectionSubmissionApplicationService { get; }
        ILookupTableApplicationService LookupTableApplicationService { get; }
        IQuestionApplicationService QuestionApplicationService { get; }
        ISectionApplicationService SectionApplicationService { get; }
    }
}
