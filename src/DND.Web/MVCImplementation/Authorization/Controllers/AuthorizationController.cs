using DND.Common.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.Authorization.Controllers
{
    public class AuthorizationController : BaseController
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
