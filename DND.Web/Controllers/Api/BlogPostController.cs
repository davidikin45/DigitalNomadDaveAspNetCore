using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers.Api;
using Solution.Base.Email;

namespace DND.Web.Controllers.Api
{
    [Route("api/blog-post")]
    public class BlogPostController : BaseEntityWebApiControllerAuthorize<BlogPostDTO,IBlogPostService>
    {
        public BlogPostController(IBlogPostService service, IMapper mapper, IEmailService emailService)
            :base(service,mapper, emailService)
        {

        }
    }
}
