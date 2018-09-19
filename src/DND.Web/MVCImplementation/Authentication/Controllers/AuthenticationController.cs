using DND.Common.Controllers;
using DND.Common.OpenIDConnect;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.Authentication.Controllers
{
    [Authorize]
    public class AuthenticationController : MvcControllerBase
    {
        public async Task Logout()
        {
            var openIDConnectClient = new OpenIDConnectClient("https://localhost:44318/", "mvc_client", "secret");
            await openIDConnectClient.LogOut(HttpContext);
        }
    }
}
