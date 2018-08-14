using DND.Common.Interfaces.ApplicationServices;

namespace DND.Interfaces.DynamicForms.ApplicationServices
{
    public interface IDynamicFormsApplicationServices : IBaseApplicationService
    {
        IFormApplicationService FormApplicationService { get; }
        IFormSubmissionApplicationService FormSubmissionApplicationService { get; }
        IFormSectionSubmissionApplicationService FormSectionSubmissionApplicationService { get; }
        ILookupTableApplicationService LookupTableApplicationService { get; }
        IQuestionApplicationService QuestionApplicationService { get; }
        ISectionApplicationService SectionApplicationService { get; }
    }
}
