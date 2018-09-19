using DND.Common.DomainServices;
using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Common.Infrastructure.Validation;
using DND.Data.DynamicForms;
using DND.Domain.DynamicForms.Sections;
using DND.Interfaces.DynamicForms.DomainServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.DynamicForms.Sections.Services
{
    public class SectionDomainService : DomainServiceEntityBase<DynamicFormsContext, Section>, ISectionDomainService
    {
        public SectionDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public override Task<Result<Section>> CreateAsync(CancellationToken cancellationToken, Section entity, string createdBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return base.CreateAsync(cancellationToken, entity, createdBy);
        }

        public override Task<Result> UpdateAsync(CancellationToken cancellationToken, Section entity, string updatedBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return base.UpdateAsync(cancellationToken, entity, updatedBy);
        }
    }
}
