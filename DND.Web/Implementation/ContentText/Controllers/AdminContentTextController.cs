using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.CMS.ContentTexts.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Implementation.Category.Controllers
{
    [Route("admin/content-text")]
    public class AdminContentTextController : BaseEntityControllerAuthorize<ContentTextDto, IContentTextApplicationService>
    {
        public AdminContentTextController(IContentTextApplicationService service, IMapper mapper, IEmailService emailService)
             : base(true, service, mapper, emailService)
        {
        }


    }
}
