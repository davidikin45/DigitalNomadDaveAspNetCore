using DND.Common.DomainServices;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Common.Infrastructure.Validation;
using DND.Common.Infrastructure.Validation.Errors;
using DND.Data;
using DND.Domain.CMS.ContentHtmls;
using DND.Interfaces.CMS.DomainServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.CMS.ContentHtmls.Services
{
    public class ContentHtmlDomainService : DomainServiceEntityBase<ApplicationContext, ContentHtml>, IContentHtmlDomainService
    {
        public ContentHtmlDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async override Task<Result> DeleteAsync(CancellationToken cancellationToken, ContentHtml entity, string deletedBy)
        {
            var dbEntity = await GetByIdAsync(entity.Id, cancellationToken);
            if (dbEntity != null && dbEntity.PreventDelete)
            {
                return Result.ObjectValidationFail("This CMS content cannot be deleted");
            }

            return await base.DeleteAsync(cancellationToken, entity, deletedBy);
        }
    }
}
