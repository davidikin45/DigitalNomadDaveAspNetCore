using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.CMS.Faqs.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.MVCImplementation.Faq.Controllers
{
    [Route("admin/faq")]
    public class AdminFaqController : BaseEntityControllerAuthorize<FaqDto, FaqDto, FaqDto, FaqDeleteDto, IFaqApplicationService>
    {
        public AdminFaqController(IFaqApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }
    }
}
