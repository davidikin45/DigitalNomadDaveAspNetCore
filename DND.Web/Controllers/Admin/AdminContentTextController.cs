using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers;
using Solution.Base.Email;

namespace DND.Web.Controllers.Admin
{
    [Route("admin/content-text")]
    public class AdminContentTextController : BaseEntityControllerAuthorize<ContentTextDTO, IContentTextApplicationService>
    {
        public AdminContentTextController(IContentTextApplicationService service, IMapper mapper, IEmailService emailService)
             : base(true, service, mapper, emailService)
        {
        }


    }
}
