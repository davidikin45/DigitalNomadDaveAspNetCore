using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Controllers.Api
{
    [ApiVersion("1.0")]
    [Route("api/blog-post")]
    public class BlogPostController : BaseEntityWebApiControllerAuthorize<BlogPostDto, IBlogPostApplicationService>
    {
        public BlogPostController(IBlogPostApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService)
            : base(service, mapper, emailService, urlHelper, typeHelperService)
        {

        }
    }
}
