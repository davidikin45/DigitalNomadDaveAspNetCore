using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.Blog.Authors.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Implementation.Author.Api
{
    [ApiVersion("1.0")]
    [Route("api/author")]
    public class AuthorController : BaseEntityWebApiControllerAuthorize<AuthorDto, AuthorDto, AuthorDto, AuthorDto, IAuthorApplicationService>
    {
        public AuthorController(IAuthorApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService)
            :base(service, mapper, emailService, urlHelper, typeHelperService)
        {

        }
    }
}
