using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.DynamicForms.Forms;
using DND.Interfaces.DynamicForms.Data;
using DND.Interfaces.DynamicForms.DomainServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.DynamicForms.Forms.Services
{
    public class FormDomainService : BaseEntityDomainService<IDynamicFormsDbContext, Form>, IFormDomainService
    {
        public FormDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public Task<Form> GetFormByUrlSlugAsync(string formUrlSlug, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
