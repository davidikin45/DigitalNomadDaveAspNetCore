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

        public override Task<Result> DeleteAsync(ContentHtml entity, CancellationToken cancellationToken)
        {
            if(entity.PreventDelete)
            {
               throw new ServiceValidationErrors(new GeneralError("This CMS content cannot be deleted"));
            }

            return base.DeleteAsync(entity, cancellationToken);
        }
    }
}
