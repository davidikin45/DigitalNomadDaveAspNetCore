using DND.Common.Implementation.DomainServices;
using DND.Common.Implementation.Validation;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.DynamicForms.Sections;
using DND.Interfaces.DynamicForms.Data;
using DND.Interfaces.DynamicForms.DomainServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.DynamicForms.Sections.Services
{
    public class SectionDomainService : BaseEntityDomainService<IDynamicFormsDbContext, Section>, ISectionDomainService
    {
        public SectionDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public override Task<Result<Section>> CreateAsync(Section entity, string createdBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return base.CreateAsync(entity, createdBy, cancellationToken);
        }

        public override Task<Result> UpdateAsync(Section entity, string updatedBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return base.UpdateAsync(entity, updatedBy, cancellationToken);
        }
    }
}
