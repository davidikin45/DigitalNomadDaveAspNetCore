using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Common.Infrastructure.Validation;
using DND.Domain.CMS.ContentHtmls;
using DND.Domain.CMS.ContentHtmls.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Interfaces.CMS.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.CMS.ContentHtmls.Services
{
    public class ContentHtmlApplicationService : ApplicationServiceEntityBase<ContentHtml, ContentHtmlDto, ContentHtmlDto, ContentHtmlDto, ContentHtmlDeleteDto, IContentHtmlDomainService>, IContentHtmlApplicationService
    {
        public ContentHtmlApplicationService(IContentHtmlDomainService domainService, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IHubContext<ApiNotificationHub<ContentHtmlDto>> hubContext)
        : base("cms.content-htmls.", domainService, mapper, authorizationService, userService, hubContext)
        {
           
        }

        public override Task<Result> DeleteAsync(ContentHtmlDeleteDto dto, string deletedBy, CancellationToken cancellationToken)
        {
            return base.DeleteAsync(dto, deletedBy, cancellationToken);
        }
    }
}
