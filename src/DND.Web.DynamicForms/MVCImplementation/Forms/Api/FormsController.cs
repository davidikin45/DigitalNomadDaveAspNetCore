﻿using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.DynamicForms.Forms.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.DynamicForms.MVCImplementation.Forms.Api
{
    [ApiVersion("1.0")]
    [Route("api/forms/forms")]
    public class FormsController : BaseEntityWebApiControllerAuthorize<FormDto, FormDto, FormDto, FormDeleteDto, IFormApplicationService>
    {
        public FormsController(IFormApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, IConfiguration configuration)
            : base(service, mapper, emailService, urlHelper, typeHelperService, configuration)
        {

        }
    }
}