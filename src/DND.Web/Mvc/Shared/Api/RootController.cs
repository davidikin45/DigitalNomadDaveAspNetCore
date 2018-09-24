using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Dtos;
using DND.Common.Infrastructure.Email;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DND.Web.Mvc.Shared.Api
{
    [ApiVersion("1.0")]
    [Route("api")]
    public class RootController : ApiControllerBase
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
                {"Authors","authors"},
                {"BlogPosts","blog_posts"},
                {"CarouselItems","carousel_items"},
                {"Categories","categories"},
                {"ContentHtmls","content_htmls"},
                {"ContentTexts","content_texts"},
                {"Faqs","faqs"},
                {"Locations","locations"},
                {"MailingList","mailing_list"},
                {"Projects","projects"},
                {"Tags","tags"},
                {"Testimonials","testimonials"},
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
