using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Email;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Services;
using DND.Common.Interfaces.Dtos;
using Microsoft.Extensions.Configuration;

namespace DND.Common.Controllers.Api
{

    //Edit returns a view of the resource being edited, the Update updates the resource it self

    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.

    //[Authorize(Roles = "admin")]
    [AllowAnonymous] // 40
    public abstract class BaseEntityReadOnlyWebApiController<TDto, IEntityService> : BaseEntityReadOnlyWebApiControllerAuthorize<TDto, IEntityService>
        where TDto : class, IBaseDtoWithId, IBaseDtoConcurrencyAware
        where IEntityService : IBaseEntityReadOnlyApplicationService<TDto>
    {   

        public BaseEntityReadOnlyWebApiController(IEntityService service, IMapper mapper = null, IEmailService emailService = null, IUrlHelper urlHelper = null, ITypeHelperService typeHelperService = null, IConfiguration configuration = null)
        : base(service, mapper, emailService, urlHelper, typeHelperService, configuration)
        {
 
        }

    }
}

