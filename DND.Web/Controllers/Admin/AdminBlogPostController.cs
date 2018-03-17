using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers;
using Solution.Base.Email;

namespace DND.Web.Controllers.Admin
{
    [Route("admin/blog-post")]
    public class AdminBlogPostController : BaseEntityControllerAuthorize<BlogPostDTO, IBlogPostApplicationService>
    {
        public AdminBlogPostController(IBlogPostApplicationService service, IMapper mapper, IEmailService emailService)
             : base(true, service, mapper, emailService)
        {
        }
    }
}
