using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Solution.Base.Email;
using Solution.Base.Interfaces.Models;
using Solution.Base.Interfaces.Services;

namespace Solution.Base.Controllers.Api
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
    public abstract class BaseEntityWebApiControllerAuthorize<TDto, IEntityService> : BaseEntityWebApiController<TDto, IEntityService>
        where TDto : class, IBaseEntity
        where IEntityService : IBaseEntityService<TDto>
    {   

        public BaseEntityWebApiControllerAuthorize(IEntityService service, IMapper mapper = null, IEmailService emailService = null)
        : base(service, mapper , emailService)
        {
          
        }
    }
}

