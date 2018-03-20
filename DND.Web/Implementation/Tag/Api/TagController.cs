using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.Blog.Tags.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Implementation.Tag.Api
{
    [ApiVersion("1.0")]
    [Route("api/tag")]
    public class TagController : BaseEntityWebApiControllerAuthorize<TagDto, ITagApplicationService>
    {
        public TagController(ITagApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService)
            : base(service, mapper, emailService, urlHelper, typeHelperService)
        {

        }

    }
}
