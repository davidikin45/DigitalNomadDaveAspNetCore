using DND.Common.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.MVCImplementation.Authorization.Controllers
{
    public class AuthorizationController : MvcControllerBase
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
