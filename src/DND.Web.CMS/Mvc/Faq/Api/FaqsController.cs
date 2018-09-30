using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Common.Interfaces.Services;
using DND.Domain.CMS.Faqs.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.CMS.Mvc.Faq.Api
{
    [ApiVersion("1.0")]
    [Route("api/cms/faqs")]
    public class FaqsController : ApiControllerEntityAuthorizeBase<FaqDto, FaqDto, FaqDto, FaqDeleteDto, IFaqApplicationService>
    {
        public FaqsController(IFaqApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, AppSettings appSettings)
            : base(service, mapper, emailService, urlHelper, typeHelperService, appSettings)
        {

        }
    }
}
