using DND.Common.Implementation.DomainServices;
using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.Persistance;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.CMS.ContentHtmls;
using DND.Domain.Interfaces.DomainServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.CMS.ContentHtmls.Services
{
    public class ContentHtmlDomainService : BaseEntityDomainService<IBaseDbContext, ContentHtml>, IContentHtmlDomainService
    {
        public ContentHtmlDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async override Task<Result> DeleteAsync(ContentHtml entity, CancellationToken cancellationToken)
        {
            var dbEntity = await GetByIdAsync(entity.Id);
            if (dbEntity != null && dbEntity.PreventDelete)
            {
               throw new ServiceValidationErrors(new GeneralError("This CMS content cannot be deleted"));
            }

            return await base.DeleteAsync(entity, cancellationToken);
        }
    }
}
