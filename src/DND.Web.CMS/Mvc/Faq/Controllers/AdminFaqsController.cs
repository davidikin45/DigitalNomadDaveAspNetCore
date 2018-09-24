using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Infrastructure.Email;
using DND.Domain.CMS.Faqs.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.CMS.Mvc.Faq.Controllers
{
    [Route("admin/cms/faqs")]
    public class AdminFaqsController : MvcControllerEntityAuthorizeBase<FaqDto, FaqDto, FaqDto, FaqDeleteDto, IFaqApplicationService>
    {
        public AdminFaqsController(IFaqApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }
    }
}
