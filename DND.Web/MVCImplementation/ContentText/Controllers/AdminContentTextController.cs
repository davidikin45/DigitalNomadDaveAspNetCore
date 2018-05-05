using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.CMS.ContentTexts.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.MVCImplementation.Category.Controllers
{
    [Route("admin/content-text")]
    public class AdminContentTextController : BaseEntityControllerAuthorize<ContentTextDto, ContentTextDto, ContentTextDto, ContentTextDeleteDto, IContentTextApplicationService>
    {
        public AdminContentTextController(IContentTextApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }


    }
}
