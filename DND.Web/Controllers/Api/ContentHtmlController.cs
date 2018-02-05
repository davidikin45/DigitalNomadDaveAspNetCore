using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers.Api;
using Solution.Base.Email;

namespace DND.Web.Controllers.Api
{
    [ApiVersion("1.0")]
    [Route("api/content-html")]
    public class ContentHtmlController : BaseEntityWebApiControllerAuthorize<ContentHtmlDTO, IContentHtmlService>
    {
        public ContentHtmlController(IContentHtmlService service, IMapper mapper, IEmailService emailService)
            :base(service,mapper, emailService)
        {

        }
    }
}
