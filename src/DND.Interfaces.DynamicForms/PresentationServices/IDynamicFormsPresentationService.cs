using DND.Common.DynamicForms;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.DynamicForms.PresentationServices
{
    public interface IDynamicFormsPresentationService
    {
        Task<DynamicFormModel> CreateFormModelFromDbAsync(string formUrlSlug, string sectionUrlSlug, CancellationToken cancellationToken = default(CancellationToken));
        Task PopulateFormModelFromDbAsync(DynamicFormModel formModel, string formSubmissionId, string sectionSlug, CancellationToken cancellationToken = default(CancellationToken));

    }
}
