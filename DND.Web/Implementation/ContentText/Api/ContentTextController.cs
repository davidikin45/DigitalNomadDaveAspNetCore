using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.CMS.ContentTexts.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Implementation.Category.Api
{
    [ApiVersion("1.0")]
    [Route("api/content-texts")]
    public class ContentTextsController : BaseEntityWebApiControllerAuthorize<ContentTextDto, ContentTextDto, ContentTextDto, ContentTextDto, IContentTextApplicationService>
    {
        public ContentTextsController(IContentTextApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService)
            : base(service, mapper, emailService, urlHelper, typeHelperService)
        {

        }
    }
}
