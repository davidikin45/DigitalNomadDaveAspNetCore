using AutoMapper;
using DND.Common.Dtos.Authentication;
using DND.Common.Extensions;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DND.Common.Controllers.Api
{
    public abstract class ApiControllerRegistrationBase<TUser, TRegistrationDto> : ApiControllerAuthenticationBase<TUser>
        where TUser : IdentityUser
        where TRegistrationDto : RegisterDto
    {
        private readonly UserManager<TUser> _userManager;

        private readonly string _welcomeEmailTemplate;

        public ApiControllerRegistrationBase(
            RoleManager<IdentityRole> roleManager,
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            TokenSettings tokenSettings,
            IUrlHelper urlHelper,
            IEmailService emailSender,
            IMapper mapper,
            PasswordSettings passwordSettings,
            EmailTemplates emailTemplates)
            :base(roleManager, userManager, signInManager, tokenSettings, urlHelper, emailSender, mapper, passwordSettings, emailTemplates)
        {
            _welcomeEmailTemplate = emailTemplates.Welcome;
        }

        #region Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] TRegistrationDto registerDto)
        {
            var user = Mapper.Map<TUser>(registerDto);
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(_welcomeEmailTemplate))
                {
                    await EmailService.SendWelcomeEmailAsync(_welcomeEmailTemplate, user.Email);
                }
                return await GenerateJWTToken(user);
            }
            AddErrors(result);
            return ValidationErrors();
        }
        #endregion
    }
}
