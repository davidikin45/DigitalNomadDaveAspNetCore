using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.DynamicForms
{
    public interface IDynamicFormsPresentationService
    {
        Task<string> GetFirstSectionUrlSlugAsync(string formUrlSlug, CancellationToken cancellationToken = default(CancellationToken));
        Task<DynamicFormModel> CreateFormModelFromDbAsync(string formUrlSlug, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken));
        Task PopulateFormModelFromDbAsync(DynamicFormModel formModel, string formSubmissionId, string sectionSlug, CancellationToken cancellationToken = default(CancellationToken));
    }
}
