using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Email;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Services;
using DND.Common.Interfaces.Dtos;

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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")] // 40
    public abstract class BaseEntityWebApiControllerAuthorize<TCreateDto, TReadDto, TUpdateDto, TDeleteDto, IEntityService> : BaseEntityWebApiController<TCreateDto, TReadDto, TUpdateDto, TDeleteDto, IEntityService>
        where TCreateDto : class, IBaseDto
        where TReadDto : class, IBaseDtoWithId
        where TUpdateDto : class, IBaseDto
        where TDeleteDto : class, IBaseDtoWithId
        where IEntityService : IBaseEntityApplicationService<TCreateDto, TReadDto, TUpdateDto, TDeleteDto>
    {   

        public BaseEntityWebApiControllerAuthorize(IEntityService service, IMapper mapper = null, IEmailService emailService = null, IUrlHelper urlHelper = null, ITypeHelperService typeHelperService = null)
        : base(service, mapper , emailService, urlHelper, typeHelperService)
        {
          
        }
    }
}

