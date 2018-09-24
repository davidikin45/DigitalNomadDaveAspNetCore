using DND.Common.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Mvc.Authorization.Controllers
{
    public class AuthorizationController : MvcControllerBase
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
