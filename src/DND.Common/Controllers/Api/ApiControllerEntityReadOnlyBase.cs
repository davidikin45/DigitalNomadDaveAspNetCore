using AutoMapper;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Common.Infrastructure.Settings;
using DND.Common.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public abstract class ApiControllerEntityReadOnlyBase<TDto, IEntityService> : ApiControllerEntityReadOnlyAuthorizeBase<TDto, IEntityService>
        where TDto : class
        where IEntityService : IApplicationServiceEntityReadOnly<TDto>
    {   

        public ApiControllerEntityReadOnlyBase(string resource, IEntityService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, AppSettings appSettings, IAuthorizationService authorizationService)
        : base(resource, service, mapper, emailService, urlHelper, typeHelperService, appSettings, authorizationService)
        {
 
        }

    }
}

