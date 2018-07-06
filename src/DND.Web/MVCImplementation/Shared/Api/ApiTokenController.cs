using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Extensions;
using DND.Common.JwtTokens;
using DND.Domain.Identity.Users;
using DND.Web.MVCImplementation.Account.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.Shared.Api
{
    [ApiVersion("1.0")]
    [Route("api/apitoken")]
    public class ApiTokenController : BaseWebApiController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _privateSigningKeyPath;

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
        }

        //[FromBody]
        [HttpPost("FullAccessToken")]
        [AllowAnonymous]
        public async Task<IActionResult> FullAccessToken([FromBody] LoginViewModel model)
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


                        var key = SigningKey.LoadPrivateRsaSigningKey(_privateSigningKeyPath);

                        var results = JwtTokenHelper.CreateJwtTokenSigningWithRsaSecurityKey(user.Email, user.UserName, roles, 20, key, _configuration["Tokens:Issuer"], ApiScopes.Full);

                        return Created("", results);
                    }
                }

            }

            return BadRequest();
        }

        [HttpPost("CreateAccessToken")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAccessToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                    if (result.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(user);

                        var key = SigningKey.LoadPrivateRsaSigningKey(_privateSigningKeyPath);

                        var results = JwtTokenHelper.CreateJwtTokenSigningWithRsaSecurityKey(user.Email, user.UserName, roles, 20, key, _configuration["Tokens:Issuer"], ApiScopes.Create);

                        return Created("", results);
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("ReadAccessToken")]
        [AllowAnonymous]
        public async Task<IActionResult> ReadAccessToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                    if (result.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(user);

                        var key = SigningKey.LoadPrivateRsaSigningKey(_privateSigningKeyPath);

                        var results = JwtTokenHelper.CreateJwtTokenSigningWithRsaSecurityKey(user.Email, user.UserName, roles, 20, key, _configuration["Tokens:Issuer"], ApiScopes.Read);

                        return Created("", results);
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("UpdateAccessToken")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateAccessToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                    if (result.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(user);

                        var key = SigningKey.LoadPrivateRsaSigningKey(_privateSigningKeyPath);

                        var results = JwtTokenHelper.CreateJwtTokenSigningWithRsaSecurityKey(user.Email, user.UserName, roles, 20, key, _configuration["Tokens:Issuer"], ApiScopes.Update);

                        return Created("", results);
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost("DeleteAccessToken")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteAccessToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                    if (result.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(user);

                        //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var key =  SigningKey.LoadPrivateRsaSigningKey(_privateSigningKeyPath);

                        var results = JwtTokenHelper.CreateJwtTokenSigningWithRsaSecurityKey(user.Email, user.UserName, roles, 20, key, _configuration["Tokens:Issuer"], ApiScopes.Delete);

                        return Created("", results);
                    }
                }
            }

            return BadRequest();
        }
    }
}
