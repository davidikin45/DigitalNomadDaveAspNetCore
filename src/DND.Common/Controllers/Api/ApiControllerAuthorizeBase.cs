﻿using AutoMapper;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DND.Common.Controllers.Api
{


    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    //[Authorize(Roles = "admin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")] // 40
    public abstract class ApiControllerAuthorizeBase : ApiControllerBase
    {
        public ApiControllerAuthorizeBase(string resource, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, AppSettings appSettings, IAuthorizationService authorizationService)
            :base(resource, mapper, emailService, urlHelper, appSettings, authorizationService)
        {
           
        }
    }
}

