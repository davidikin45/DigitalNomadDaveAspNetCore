using DND.Common.DomainServices;
using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Common.Infrastructure.Validation;
using DND.Data.DynamicForms;
using DND.Domain.DynamicForms.Forms;
using DND.Interfaces.DynamicForms.DomainServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.DynamicForms.Forms.Services
{
    public class FormDomainService : DomainServiceEntityBase<DynamicFormsContext, Form>, IFormDomainService
    {
        public FormDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public override Task<Result<Form>> CreateAsync(CancellationToken cancellationToken, Form entity, string createdBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return base.CreateAsync(cancellationToken, entity, createdBy);
        }

        public override Task<Result> UpdateAsync(CancellationToken cancellationToken, Form entity, string updatedBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return base.UpdateAsync(cancellationToken, entity, updatedBy);
        }
    }
}
