using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.Controllers.Api
{
    [ApiVersion("1.0")]
    [Route("api/blog-posts")]
    public class BlogPostsController : BaseEntityWebApiControllerAuthorize<BlogPostDto, BlogPostDto, BlogPostDto, BlogPostDeleteDto, IBlogPostApplicationService>
    {
        public BlogPostsController(IBlogPostApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, IConfiguration configuration)
            : base(service, mapper, emailService, urlHelper, typeHelperService, configuration)
        {

        }
    }
}
