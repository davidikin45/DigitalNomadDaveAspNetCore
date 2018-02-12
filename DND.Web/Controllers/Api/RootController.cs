using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers.Api;
using Solution.Base.Email;
using Solution.Base.Implementation.DTOs;
using Solution.Base.Interfaces.Services;
using System.Collections.Generic;

namespace DND.Web.Controllers.Api
{
    [ApiVersion("1.0")]
    [Route("api")]
    public class RootController : BaseWebApiControllerAuthorize
    {
        public RootController(IMapper mapper, IEmailService emailService, IUrlHelper urlHelper)
            : base(mapper, emailService, urlHelper)
        {

        }

        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();

            links.Add(
              new LinkDto(UrlHelper.Link("GetRoot", new { }),
              "self",
              "GET"));

            //links.Add(
            // new LinkDto(UrlHelper.Link("GetAuthors", new { }),
            // "authors",
            // "GET"));

            return Ok(links);
        }
    }
}
