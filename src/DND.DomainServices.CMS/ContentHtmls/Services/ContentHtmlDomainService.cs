using DND.Common.Implementation.DomainServices;
using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.CMS.ContentHtmls;
using DND.Interfaces.CMS.Data;
using DND.Interfaces.CMS.DomainServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.CMS.ContentHtmls.Services
{
    public class ContentHtmlDomainService : BaseEntityDomainService<ICMSDbContext, ContentHtml>, IContentHtmlDomainService
    {
        public ContentHtmlDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async override Task<Result> DeleteAsync(ContentHtml entity, string deletedBy, CancellationToken cancellationToken)
        {
            var dbEntity = await GetByIdAsync(entity.Id);
            if (dbEntity != null && dbEntity.PreventDelete)
            {
               throw new ServiceValidationErrors(new GeneralError("This CMS content cannot be deleted"));
            }

            return await base.DeleteAsync(entity, deletedBy, cancellationToken);
        }
    }
}
