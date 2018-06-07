using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.CMS.ContentTexts.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.MVCImplementation.Category.Api
{
    [ApiVersion("1.0")]
    [Route("api/content-texts")]
    public class ContentTextsController : BaseEntityWebApiControllerAuthorize<ContentTextDto, ContentTextDto, ContentTextDto, ContentTextDeleteDto, IContentTextApplicationService>
    {
        public ContentTextsController(IContentTextApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, IConfiguration configuration)
            : base(service, mapper, emailService, urlHelper, typeHelperService, configuration)
        {

        }
    }
}
