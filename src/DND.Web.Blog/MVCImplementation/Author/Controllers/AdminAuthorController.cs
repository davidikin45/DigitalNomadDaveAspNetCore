using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Email;
using DND.Domain.Blog.Authors.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.Blog.MVCImplementation.Author.Controllers
{
    [Route("admin/blog/authors")]
    public class AdminAuthorsController : BaseEntityControllerAuthorize<AuthorDto, AuthorDto, AuthorDto, AuthorDeleteDto, IAuthorApplicationService>
    {
        public AdminAuthorsController(IAuthorApplicationService service, IMapper mapper,IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService)
        {
        }
    }
}
