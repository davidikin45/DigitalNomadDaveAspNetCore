using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers.Api;
using Solution.Base.Email;
using Solution.Base.Implementation.DTOs;
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
              new LinkDto(UrlHelper.Action(nameof(GetRoot), 
              UrlHelper.ActionContext.RouteData.Values["controller"].ToString(), 
              new { },
              UrlHelper.ActionContext.HttpContext.Request.Scheme),
              "self",
              "GET"));

            var apis = new Dictionary<string, string>()
            {
                {"Author","author"},
                {"BlogPost","blog_post"},
                {"Category","category"},
                {"ContentHtml","content_html"},
                {"ContentText","content_text"},
                {"Faq","faq"},
                {"Location","location"},
                {"MailingList","mailing_list"},
                {"Project","project"},
                {"Tag","tag"},
                {"Testimonial","testimonial"},
            };

            foreach(KeyValuePair<string,string> api in apis)
            {
                links.Add(
               new LinkDto(UrlHelper.Action("GetPaged",
               api.Key,
               new { },
               UrlHelper.ActionContext.HttpContext.Request.Scheme),
              api.Value,
               "GET"));
            }

            return Ok(links);
        }
    }
}
