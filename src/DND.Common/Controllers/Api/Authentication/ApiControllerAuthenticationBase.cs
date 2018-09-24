﻿using AutoMapper;
using DND.Common.Dtos.Authentication;
using DND.Common.Extensions;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Helpers;
using DND.Common.Infrastructure.Settings;
using DND.Common.JwtTokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Common.Controllers.Api
{

    public abstract class ApiControllerAuthenticationBase<TUser> : ApiControllerBase
        where TUser : IdentityUser
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<TUser> _userManager;
        private readonly SignInManager<TUser> _signInManager;

        private readonly string _privateSymmetricKey;
        private readonly string _privateSigningKeyPath;
        private readonly string _privateSigningCertificatePath;
        private readonly string _privateSigningCertificatePassword;
        private readonly string _localIssuer;

        private readonly string _passwordResetCallbackUrl;

        private readonly string _resetPasswordEmailTemplate;

        private readonly int _tokenExpiryMinutes;

        public ApiControllerAuthenticationBase(
            RoleManager<IdentityRole> roleManager,
            UserManager<TUser> userManager,
            SignInManager<TUser> signInManager,
            IOptions<TokenSettings> tokenSettings,
            IUrlHelper urlHelper,
            IEmailService emailSender,
            IMapper mapper,
            IOptions<PasswordSettings> passwordSettings,
            IOptions<EmailTemplates> emailTemplates)
            :base(mapper, emailSender, urlHelper)
        {
            _resetPasswordEmailTemplate = emailTemplates.Value.ResetPassword;

            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;

            _privateSymmetricKey = tokenSettings.Value.Key;
            _privateSigningKeyPath = tokenSettings.Value.PrivateKeyPath;
            _privateSigningCertificatePath = tokenSettings.Value.PrivateCertificatePath;
            _privateSigningCertificatePassword = tokenSettings.Value.PrivateCertificatePasword;

            _localIssuer = tokenSettings.Value.LocalIssuer;
            _tokenExpiryMinutes = tokenSettings.Value.ExpiryMinutes;
        }


        #region HttpOptions
        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <returns></returns>
        [HttpOptions]
        public IActionResult GetOptions()
        {
            Response.Headers.Add("Allow", "GET, POST, OPTIONS");
            return Ok();
        }
        #endregion

        #region Authenticate
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateDto authenticateDto)
        {
            var user = await _userManager.FindByNameAsync(authenticateDto.Username);
            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, authenticateDto.Password, true);

                if (result.Succeeded)
                {
                    return await GenerateJWTToken(user);
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return ValidationErrors();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return ValidationErrors();
            }
        }
        #endregion

        #region Generate JWT Token
        protected async Task<IActionResult> GenerateJWTToken(TUser user)
        {
            //Add roles
            var roles = await _userManager.GetRolesAsync(user);
            var scopes = (await _userManager.GetClaimsAsync(user)).Where(c => c.Type == "scope").Select(c => c.Value).ToHashSet();

            var ownerRole = await _roleManager.FindByNameAsync("authenticated");
            if (ownerRole != null)
            {
                var roleScopes = (await _roleManager.GetClaimsAsync(ownerRole)).Where(c => c.Type == "scope").Select(c => c.Value).ToList();
                foreach (var scope in roleScopes)
                {
                    scopes.Add(scope);
                }
            }

            //Add role scopes.
            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var roleScopes = (await _roleManager.GetClaimsAsync(role)).Where(c => c.Type == "scope").Select(c => c.Value).ToList();
                    foreach (var scope in roleScopes)
                    {
                        scopes.Add(scope);
                    }

                }
            }

            if (!string.IsNullOrWhiteSpace(_privateSigningKeyPath))
            {
                var key = SigningKey.LoadPrivateRsaSigningKey(_privateSigningKeyPath);
                var results = JwtTokenHelper.CreateJwtTokenSigningWithRsaSecurityKey(user.Id, user.UserName, roles, _tokenExpiryMinutes, key, _localIssuer, "api", scopes.ToArray());
                return Created("", results);
            }
            else if (!string.IsNullOrWhiteSpace(_privateSigningCertificatePassword))
            {
                var key = SigningKey.LoadPrivateSigningCertificate(_privateSigningCertificatePassword, _privateSigningCertificatePassword);
                var results = JwtTokenHelper.CreateJwtTokenSigningWithCertificateSecurityKey(user.Id, user.UserName, roles, _tokenExpiryMinutes, key, _localIssuer, "api", scopes.ToArray());
                return Created("", results);
            }
            else
            {
                var results = JwtTokenHelper.CreateJwtTokenSigningWithKey(user.Id, user.UserName, roles, _tokenExpiryMinutes, _privateSymmetricKey, _localIssuer, "api", scopes.ToArray());
                return Created("", results);
            }
        }
        #endregion

        #region Forgot Password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByNameAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                return Ok();
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            if (!string.IsNullOrEmpty(_resetPasswordEmailTemplate))
            {
                await EmailService.SendResetPasswordEmailAsync(Url, _resetPasswordEmailTemplate, forgotPasswordDto.Email, _passwordResetCallbackUrl, user.Id, code, Request.Scheme);
            }

            return Ok();
        }
        #endregion

        #region Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByNameAsync(resetPasswordDto.Email);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Code, resetPasswordDto.Password);
                if (result.Succeeded)
                {
                    return Ok();
                }
                AddErrors(result);
                return ValidationErrors();
            }
            else
            {
                return Ok();
            }
        }
        #endregion

        #region Helpers
        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        #endregion
    }
}