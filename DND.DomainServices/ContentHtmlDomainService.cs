using DND.Domain.DTOs;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Models;
using Solution.Base.Implementation.DomainServices;
using Solution.Base.Implementation.Validation;
using Solution.Base.Interfaces.Persistance;
using Solution.Base.Interfaces.UnitOfWork;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices
{
    public class ContentHtmlDomainService : BaseEntityDomainService<IBaseDbContext, ContentHtml>, IContentHtmlDomainService
    {
        public ContentHtmlDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public override Task DeleteAsync(ContentHtml entity, CancellationToken cancellationToken)
        {
            if(entity.PreventDelete)
            {
               throw new ServiceValidationErrors(new GeneralError("This CMS content cannot be deleted"));
            }

            return base.DeleteAsync(entity, cancellationToken);
        }
    }
}
