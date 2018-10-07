using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Domain.Identity.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Mvc.Shared.Api
{
    [ApiVersion("1.0")]
    [Route("api/users")]
    public class UsersController : ApiControllerAuthenticationBase<User>
    {      
        public UsersController(
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            TokenSettings tokenSettings,
            IUrlHelper urlHelper,
            IEmailService emailSender,
            IMapper mapper,
            PasswordSettings passwordSettings,
            EmailTemplates emailTemplates,
            AppSettings appSettings,
            IAuthorizationService authorizationService)
            :base("users.", roleManager, userManager, signInManager, tokenSettings, urlHelper, emailSender, mapper, passwordSettings, emailTemplates, appSettings, authorizationService)
        {

        }       
    }
}
