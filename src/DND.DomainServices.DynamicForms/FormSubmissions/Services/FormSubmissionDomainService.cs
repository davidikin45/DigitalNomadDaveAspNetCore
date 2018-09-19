using DND.Common.DomainServices;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Data.DynamicForms;
using DND.Domain.DynamicForms.FormSubmissions;
using DND.Interfaces.DynamicForms.DomainServices;

namespace DND.DomainServices.DynamicForms.FormSubmissions.Services
{
    public class FormSubmissionDomainService : DomainServiceEntityBase<DynamicFormsContext, FormSubmission>, IFormSubmissionDomainService
    {
        public FormSubmissionDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
    }
}
