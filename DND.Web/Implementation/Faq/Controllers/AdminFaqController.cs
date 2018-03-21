using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.CMS.Faqs.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Implementation.Faq.Controllers
{
    [Route("admin/faq")]
    public class AdminFaqController : BaseEntityControllerAuthorize<FaqDto, FaqDto, FaqDto, FaqDto, IFaqApplicationService>
    {
        public AdminFaqController(IFaqApplicationService service, IMapper mapper, IEmailService emailService)
             : base(true, service, mapper, emailService)
        {
        }
    }
}
