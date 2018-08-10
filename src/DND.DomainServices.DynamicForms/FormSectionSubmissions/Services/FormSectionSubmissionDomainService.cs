using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.DynamicForms.FormSectionSubmissions;
using DND.Interfaces.DynamicForms.Data;
using DND.Interfaces.DynamicForms.DomainServices;

namespace DND.DomainServices.DynamicForms.FormSectionSubmissions.Services
{
    public class FormSectionSubmissionsDomainService : BaseEntityDomainService<IDynamicFormsDbContext, FormSectionSubmission>, IFormSectionSubmissionDomainService
    {
        public FormSectionSubmissionsDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
    }
}
