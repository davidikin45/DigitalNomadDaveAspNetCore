using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Models;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.Persistance;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices
{
    public class ContentHtmlApplicationService : BaseEntityApplicationService<IBaseDbContext, ContentHtml, ContentHtmlDTO, IContentHtmlDomainService>, IContentHtmlApplicationService
    {
        public ContentHtmlApplicationService(IContentHtmlDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {
           
        }

        public override Task DeleteAsync(ContentHtmlDTO dto, CancellationToken cancellationToken)
        {
            if(dto.PreventDelete)
            {
               throw new ServiceValidationErrors(new GeneralError("This CMS content cannot be deleted"));
            }

            return base.DeleteAsync(dto, cancellationToken);
        }
    }
}
