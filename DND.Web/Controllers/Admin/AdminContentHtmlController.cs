using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Controllers;
using DND.Common.Email;

namespace DND.Web.Controllers.Admin
{
    [Route("admin/content-html")]
    public class AdminContentHtmlController : BaseEntityControllerAuthorize<ContentHtmlDTO, IContentHtmlApplicationService>
    {
        public AdminContentHtmlController(IContentHtmlApplicationService service, IMapper mapper, IEmailService emailService)
             : base(true, service, mapper, emailService)
        {
        }

    }
}
