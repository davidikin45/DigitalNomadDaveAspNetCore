using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Infrastructure.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.Blog.Authors.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.Blog.Mvc.Author.Api
{
    [ApiVersion("1.0")]
    [Route("api/blog/authors")]
    public class AuthorsController : ApiControllerEntityAuthorizeBase<AuthorDto, AuthorDto, AuthorDto, AuthorDeleteDto, IAuthorApplicationService>
    {
        public AuthorsController(IAuthorApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, IConfiguration configuration)
            :base(service, mapper, emailService, urlHelper, typeHelperService, configuration)
        {

        }
    }
}
