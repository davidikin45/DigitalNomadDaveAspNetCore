using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.DynamicForms
{
    public interface IDynamicFormsPresentationService
    {
        Task<bool> IsValidUrl(string formUrlSlug, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken));
        Task<string> GetFirstSectionUrlSlugAsync(string formUrlSlug, CancellationToken cancellationToken = default(CancellationToken));
        Task<DynamicForm> CreateFormModelFromDbAsync(string formUrlSlug, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveFormModelToDb(DynamicForm formModel, string formSubmissionId, string formUrlSlug, string sectionUrlSlug, bool isValid, CancellationToken cancellationToken = default(CancellationToken));
        Task PopulateFormModelFromDbAsync(DynamicForm formModel, string formSubmissionId, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> IsValidSubmissionUrl(string formSubmissionId, string formUrlSlug, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken));
        Task<string> GetNextSectionUrlSlug(string formSubmissionId, string formUrlSlug, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken));

        Task<DynamicFormNavigation> GetFormNavigationAsync(string formUrlSlug, string formSubmissionId, CancellationToken cancellationToken = default(CancellationToken));

        Task<DynamicFormContainer> CreateFormContainerAsync(DynamicForm formModel, string formUrlSlug, string sectionUrlSlug, string formSubmissionId, CancellationToken cancellationToken = default(CancellationToken));
        Task<DynamicFormContainer> CreateFormSummaryContainerAsync(string formUrlSlug, string formSubmissionId, CancellationToken cancellationToken = default(CancellationToken));
        Task<DynamicFormContainer> CreateFormConfirmationContainerAsync(string formUrlSlug, string formSubmissionId, CancellationToken cancellationToken = default(CancellationToken));
    }
}

