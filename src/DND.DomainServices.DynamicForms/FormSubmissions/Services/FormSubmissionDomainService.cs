using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.DynamicForms.FormSubmissions;
using DND.Interfaces.DynamicForms.Data;
using DND.Interfaces.DynamicForms.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.DomainServices.DynamicForms.FormSubmissions.Services
{
    public class FormSubmissionDomainService : BaseEntityDomainService<IDynamicFormsDbContext, FormSubmission>, IFormSubmissionDomainService
    {
        public FormSubmissionDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }
    }
}
