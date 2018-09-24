using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
            IOptions<TokenSettings> tokenSettings,
            IUrlHelper urlHelper,
            IEmailService emailSender,
            IMapper mapper,
            IOptions<PasswordSettings> passwordSettings,
            IOptions<EmailTemplates> emailTemplates)
            :base(roleManager, userManager, signInManager, tokenSettings, urlHelper, emailSender, mapper, passwordSettings, emailTemplates)
        {

        }       
    }
}
