using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Solution.Base.Interfaces.Models;
using Solution.Base.Interfaces.Services;
using System;

namespace Solution.Base.Controllers
{

    //Edit returns a view of the resource being edited, the Update updates the resource it self

    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    [Authorize(Roles = "admin")]
    public abstract class BaseEntityReadOnlyControllerAuthorize<TDto, IEntityService> : BaseEntityReadOnlyController<TDto, IEntityService>
        where TDto : class, IBaseEntity
        where IEntityService : IBaseEntityService<TDto>
    {

        public BaseEntityReadOnlyControllerAuthorize(Boolean admin, IEntityService service, IMapper mapper = null)
        : base(admin, service, mapper)
        {

        }

    }
}

