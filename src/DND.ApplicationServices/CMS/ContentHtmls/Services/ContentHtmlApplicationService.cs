using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.Persistance;
using DND.Domain.CMS.ContentHtmls;
using DND.Domain.CMS.ContentHtmls.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.CMS.ContentHtmls.Services
{
    public class ContentHtmlApplicationService : BaseEntityApplicationService<IBaseDbContext, ContentHtml, ContentHtmlDto, ContentHtmlDto, ContentHtmlDto, ContentHtmlDeleteDto, IContentHtmlDomainService>, IContentHtmlApplicationService
    {
        public ContentHtmlApplicationService(IContentHtmlDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {
           
        }

        public override Task<Result> DeleteAsync(ContentHtmlDeleteDto dto, string deletedBy, CancellationToken cancellationToken)
        {
            return base.DeleteAsync(dto, deletedBy, cancellationToken);
        }
    }
}
