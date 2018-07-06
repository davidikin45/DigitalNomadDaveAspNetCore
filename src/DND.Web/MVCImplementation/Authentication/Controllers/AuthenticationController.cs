using DND.Common.Controllers;
using DND.Common.OpenIDConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.Authentication.Controllers
{
    [Authorize]
    public class AuthenticationController : BaseController
    {
        public async Task Logout()
        {
            var openIDConnectClient = new OpenIDConnectClient("https://localhost:44318/", "mvc_client", "secret");
            await openIDConnectClient.LogOut(HttpContext);
        }
    }
}
