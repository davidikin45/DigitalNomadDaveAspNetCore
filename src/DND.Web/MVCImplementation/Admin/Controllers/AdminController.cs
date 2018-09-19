using AutoMapper;
using DND.Common.Controllers.Admin;
using DND.Common.Infrastructure.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.MVCImplementation.Admin.Controllers
{
    //[LayoutInjector("_LayoutAdmin")]
    [Route("admin")]
    public class AdminController : MvcControllerAdminAuthorizeBase
    {

        public AdminController(IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(mapper, emailService, configuration)
        {
        }


        public override ActionResult ClearCache()
        {
            return base.ClearCache();
        }

    }
} 