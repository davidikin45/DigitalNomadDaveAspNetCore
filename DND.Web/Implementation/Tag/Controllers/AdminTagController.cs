using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.Blog.Tags.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.Implementation.Tag.Controllers
{
    [Route("admin/tag")]
    public class AdminTagController : BaseEntityControllerAuthorize<TagDto, TagDto, TagDto, TagDeleteDto, ITagApplicationService>
    {
        public AdminTagController(ITagApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }
    }
}
