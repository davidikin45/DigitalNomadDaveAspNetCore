using DND.Common.DomainServices;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Data.DynamicForms;
using DND.Domain.DynamicForms.FormSectionSubmissions;
using DND.Interfaces.DynamicForms.DomainServices;

namespace DND.DomainServices.DynamicForms.FormSectionSubmissions.Services
{
    public class FormSectionSubmissionDomainService : DomainServiceEntityBase<DynamicFormsContext, FormSectionSubmission>, IFormSectionSubmissionDomainService
    {
        public FormSectionSubmissionDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
    }
}
