using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Controllers.Admin;
using Microsoft.Extensions.Configuration;
using DND.Common.Email;

namespace DND.Web.Implementation.Admin.Controllers
{
    //[LayoutInjector("_LayoutAdmin")]
    [Route("admin")]
    public class AdminController : BaseAdminControllerAuthorize
    {

        public AdminController(IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(mapper, emailService, configuration)
        {
        }

    }
}