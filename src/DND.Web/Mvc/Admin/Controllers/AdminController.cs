using AutoMapper;
using DND.Common.Controllers.Admin;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.Mvc.Admin.Controllers
{
    //[LayoutInjector("_LayoutAdmin")]
    [Route("admin")]
    public class AdminController : MvcControllerAdminAuthorizeBase
    {

        public AdminController(IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(mapper, emailService, appSettings)
        {
        }


        public override ActionResult ClearCache()
        {
            return base.ClearCache();
        }

    }
} 