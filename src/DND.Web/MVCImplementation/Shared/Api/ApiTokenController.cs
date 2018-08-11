﻿using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Extensions;
using DND.Common.Helpers.Cryptography;
using DND.Common.JwtTokens;
using DND.Domain.Identity.Users;
using DND.Web.MVCImplementation.Account.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.Shared.Api
{
    [ApiVersion("1.0")]
    [Route("api/api-token")]
    public class ApiTokenController : BaseWebApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _privateSigningKeyPath;
        private readonly string _privateSigningCertificatePath;
        private readonly string _privateSigningCertificatePassword;

        public ApiTokenController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            IMapper mapper, 
            IEmailService emailService,
            IUrlHelper urlHelper)
            :base(mapper, emailService, urlHelper)
        {
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;

            _privateSigningKeyPath = _hostingEnvironment.MapContentPath(_configuration.GetValue<string>("Tokens:PrivateKeyPath"));
            _privateSigningCertificatePath = _hostingEnvironment.MapContentPath(_configuration.GetValue<string>("Tokens:PrivateCertificatePath"));
            _privateSigningCertificatePassword = _hostingEnvironment.MapContentPath(_configuration.GetValue<string>("Tokens:PrivateCertificatePassword"));
        }

        //[FromBody]
        [HttpPost("access-token")]
        [AllowAnonymous]
        public async Task<IActionResult> AccessToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                    if (result.Succeeded)
                    {
                        //Add roles
                        var roles = await _userManager.GetRolesAsync(user);
                        var scopes = (await _userManager.GetClaimsAsync(user)).Where(c => c.Type == "scope").Select(c => c.Value);

                        if(!string.IsNullOrWhiteSpace(_privateSigningKeyPath))
                        {
                            var key = SigningKey.LoadPrivateRsaSigningKey(_privateSigningKeyPath);
                            var results = JwtTokenHelper.CreateJwtTokenSigningWithRsaSecurityKey(user.Email, user.UserName, roles, 20, key, _configuration["Tokens:LocalIssuer"], "api", scopes.ToArray());
                            return Created("", results);
                        }
                        else
                        {
                            var key = SigningKey.LoadPrivateSigningCertificate(_privateSigningCertificatePath, _privateSigningCertificatePassword);
                            var results = JwtTokenHelper.CreateJwtTokenSigningWithCertificateSecurityKey(user.Email, user.UserName, roles, 20, key, _configuration["Tokens:LocalIssuer"], "api", scopes.ToArray());
                            return Created("", results);
                        }
                    }
                }

            }

            return BadRequest();
        }
    }
}
