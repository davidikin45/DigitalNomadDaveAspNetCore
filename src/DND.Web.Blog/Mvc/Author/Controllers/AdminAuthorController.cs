using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Domain.Blog.Authors.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.Blog.Mvc.Author.Controllers
{
    [Route("admin/blog/authors")]
    public class AdminAuthorsController : MvcControllerEntityAuthorizeBase<AuthorDto, AuthorDto, AuthorDto, AuthorDeleteDto, IAuthorApplicationService>
    {
        public AdminAuthorsController(IAuthorApplicationService service, IMapper mapper,IEmailService emailService, AppSettings appSettings)
             : base(true, service, mapper, emailService)
        {
        }
    }
}
