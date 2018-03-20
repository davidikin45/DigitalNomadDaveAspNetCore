using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.Blog.Authors.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Implementation.Author.Controllers
{
    [Route("admin/author")]
    public class AdminAuthorController : BaseEntityControllerAuthorize<AuthorDto, IAuthorApplicationService>
    {
        public AdminAuthorController(IAuthorApplicationService service, IMapper mapper,IEmailService emailService)
             : base(true, service, mapper, emailService)
        {
        }
    }
}
