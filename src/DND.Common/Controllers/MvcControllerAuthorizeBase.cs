using AutoMapper;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace DND.Common.Controllers
{
    [Authorize(Roles = "admin")]
    public abstract class MvcControllerAuthorizeBase : MvcControllerBase
    {
        public MvcControllerAuthorizeBase()
        {

        }

        public MvcControllerAuthorizeBase(IMapper mapper = null, IEmailService emailService = null, AppSettings appSettings = null)
            :base(mapper, emailService, appSettings)
        {
         
        }
    }
}
