using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Controllers;
using DND.Common.Email;

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
