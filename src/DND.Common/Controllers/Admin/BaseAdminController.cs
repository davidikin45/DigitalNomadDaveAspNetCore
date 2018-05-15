using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Middleware;
using System;
using DND.Common.Email;
using Microsoft.Extensions.Configuration;

namespace DND.Common.Controllers.Admin
{
    public abstract class BaseAdminControllerAuthorize : BaseControllerAuthorize
    {

        public BaseAdminControllerAuthorize(IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(mapper, emailService, configuration)
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
