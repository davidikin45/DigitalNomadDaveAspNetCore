using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.Blog.Authors.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.MVCImplementation.Author.Controllers
{
    [Route("admin/author")]
    public class AdminAuthorController : BaseEntityControllerAuthorize<AuthorDto, AuthorDto, AuthorDto, AuthorDeleteDto, IAuthorApplicationService>
    {
        public AdminAuthorController(IAuthorApplicationService service, IMapper mapper,IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService)
        {
        }
    }
}
