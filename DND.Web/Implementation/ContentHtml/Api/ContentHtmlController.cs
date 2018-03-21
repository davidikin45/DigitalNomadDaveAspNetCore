using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.CMS.ContentHtmls.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Implementation.Category.Api
{
    [ApiVersion("1.0")]
    [Route("api/content-html")]
    public class ContentHtmlController : BaseEntityWebApiControllerAuthorize<ContentHtmlDto, ContentHtmlDto, ContentHtmlDto, ContentHtmlDto, IContentHtmlApplicationService>
    {
        public ContentHtmlController(IContentHtmlApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService)
            : base(service, mapper, emailService, urlHelper, typeHelperService)
        {

        }
    }
}
