using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.Blog.Tags.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.Blog.MVCImplementation.Tag.Controllers
{
    [Route("admin/blog/tags")]
    public class AdminTagsController : BaseEntityControllerAuthorize<TagDto, TagDto, TagDto, TagDeleteDto, ITagApplicationService>
    {
        public AdminTagsController(ITagApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }
    }
}
