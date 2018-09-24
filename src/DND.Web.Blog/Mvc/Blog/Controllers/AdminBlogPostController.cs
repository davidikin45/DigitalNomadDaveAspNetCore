using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Infrastructure.Email;
using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.Blog.Mvc.Blog.Controllers
{
    [Route("admin/blog/blog-posts")]
    public class AdminBlogPostsController : MvcControllerEntityAuthorizeBase<BlogPostDto, BlogPostDto, BlogPostDto, BlogPostDeleteDto, IBlogPostApplicationService>
    {
        public AdminBlogPostsController(IBlogPostApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }
    }
}
