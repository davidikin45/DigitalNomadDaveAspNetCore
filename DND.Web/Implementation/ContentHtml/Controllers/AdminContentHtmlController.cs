using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.CMS.ContentHtmls.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Implementation.Category.ContentHtml
{
    [Route("admin/content-html")]
    public class AdminContentHtmlController : BaseEntityControllerAuthorize<ContentHtmlDto, ContentHtmlDto, ContentHtmlDto, ContentHtmlDto, IContentHtmlApplicationService>
    {
        public AdminContentHtmlController(IContentHtmlApplicationService service, IMapper mapper, IEmailService emailService)
             : base(true, service, mapper, emailService)
        {
        }

    }
}
