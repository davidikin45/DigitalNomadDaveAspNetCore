using AutoMapper;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Common.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Common.Controllers.Admin
{
    public abstract class MvcControllerAdminAuthorizeBase : MvcControllerAuthorizeBase
    {

        public MvcControllerAdminAuthorizeBase(IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(mapper, emailService, appSettings)
        {
        }

        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("file-manager")]
        public ActionResult FileManager()
        {
            return View();
        }

        //https://stackoverflow.com/questions/565239/any-way-to-clear-flush-remove-outputcache/16038654
        [Route("clear-cache")]
        public virtual ActionResult ClearCache()
        {
            ResponseCachingCustomMiddleware.ClearResponseCache();
            return View();
        }
    }
}
