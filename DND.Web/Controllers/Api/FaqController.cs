using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers.Api;
using Solution.Base.Email;

namespace DND.Web.Controllers.Api
{
    [Route("api/faq")]
    public class FaqController : BaseEntityWebApiControllerAuthorize<FaqDTO, IFaqService>
    {
        public FaqController(IFaqService service, IMapper mapper, IEmailService emailService)
            :base(service, mapper, emailService)
        {

        }
    }
}
