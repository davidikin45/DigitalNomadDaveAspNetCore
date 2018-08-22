using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.DynamicForms
{
    public interface IDynamicFormsPresentationService
    {
        Task<bool> IsValidUrlAsync(string formUrlSlug, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken));
        Task<DynamicForm> CreateFormModelFromDbAsync(string formUrlSlug, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveFormModelToDbAsync(DynamicForm formModel, string formSubmissionId, string formUrlSlug, string sectionUrlSlug, bool isValid, CancellationToken cancellationToken = default(CancellationToken));
        Task PopulateFormModelFromDbAsync(DynamicForm formModel, string formSubmissionId, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> GetFirstSectionUrlSlugAsync(string formUrlSlug, string controllerName, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> IsValidSubmissionUrlAsync(string formSubmissionId, string formUrlSlug, string sectionUrlSlug, string controllerName, CancellationToken cancellationToken = default(CancellationToken));
        Task<string> GetNextSectionUrlSlugAsync(string formSubmissionId, string formUrlSlug, string sectionUrlSlug, string controllerName, CancellationToken cancellationToken = default(CancellationToken));

        Task<DynamicFormNavigation> GetFormNavigationAsync(string formUrlSlug, string formSubmissionId, string controllerName, CancellationToken cancellationToken = default(CancellationToken));

        Task<DynamicFormContainer> CreateFormContainerAsync(DynamicForm formModel, string formUrlSlug, string sectionUrlSlug, string formSubmissionId, string controllerName, CancellationToken cancellationToken = default(CancellationToken));
        Task<DynamicFormContainer> CreateFormSummaryContainerAsync(string formUrlSlug, string formSubmissionId, string containerDiv, string controllerName, CancellationToken cancellationToken = default(CancellationToken));
        Task<DynamicFormContainer> CreateFormConfirmationContainerAsync(string formUrlSlug, string formSubmissionId, string controllerName, CancellationToken cancellationToken = default(CancellationToken));
    }
}

