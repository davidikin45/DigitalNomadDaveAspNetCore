using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Common.Interfaces.Services;
using DND.Domain.CMS.MailingLists.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.CMS.Mvc.MailingList.Api
{
    [ApiVersion("1.0")]
    [Route("api/cms/mailing-list")]
    public class MailingListController : ApiControllerEntityAuthorizeBase<MailingListDto, MailingListDto, MailingListDto, MailingListDeleteDto, IMailingListApplicationService>
    {
        public MailingListController(IMailingListApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, AppSettings appSettings)
            : base(service, mapper, emailService, urlHelper, typeHelperService, appSettings)
        {

        }
    }
}
