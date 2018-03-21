using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using DND.Common.Email;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.Models;
using System;
using DND.Common.Interfaces.Dtos;

namespace DND.Common.Controllers
{

    //Edit returns a view of the resource being edited, the Update updates the resource it self

    //C - Create - POST
    //R - Read - GET
    //U - Update - PUTs
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    [Authorize(Roles = "admin")]
    public abstract class BaseEntityControllerAuthorize<TCreate, TRead, TUpdate, TDelete, IEntityService> : BaseEntityController<TCreate, TRead, TUpdate, TDelete, IEntityService>
        where TCreate : class, IBaseDto
        where TRead : class, IBaseDtoWithId
        where TUpdate : class, IBaseDto
        where TDelete : class, IBaseDtoWithId
        where IEntityService : IBaseEntityApplicationService<TCreate, TRead, TUpdate, TDelete>
    {
        public BaseEntityControllerAuthorize(Boolean admin, IEntityService service, IMapper mapper = null, IEmailService emailService = null)
        : base(admin, service, mapper, emailService)
        {
        }

    
    }
}

