using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.DynamicForms
{
    public interface IDynamicFormsPresentationService
    {
        Task<bool> IsValidUrl(string formUrlSlug, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> GetFirstSectionUrlSlugAsync(string formUrlSlug, CancellationToken cancellationToken = default(CancellationToken));
        Task<DynamicForm> CreateFormModelFromDbAsync(string formUrlSlug, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken));
        Task PopulateFormModelFromDbAsync(DynamicForm formModel, string formSubmissionId, string sectionSlug, CancellationToken cancellationToken = default(CancellationToken));
    }
}
