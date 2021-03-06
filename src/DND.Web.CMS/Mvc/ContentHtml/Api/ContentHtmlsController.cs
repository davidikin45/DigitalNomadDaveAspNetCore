﻿using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Common.Interfaces.Services;
using DND.Domain.CMS.ContentHtmls.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.CMS.Mvc.ContentHtml.Api
{
    [ApiVersion("1.0")]
    [Route("api/cms/content-htmls")]
    public class ContentHtmlsController : ApiControllerEntityAuthorizeBase<ContentHtmlDto, ContentHtmlDto, ContentHtmlDto, ContentHtmlDeleteDto, IContentHtmlApplicationService>
    {
        public ContentHtmlsController(IContentHtmlApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, AppSettings appSettings, IAuthorizationService authorizationService)
            : base("cms.content-htmls.", service, mapper, emailService, urlHelper, typeHelperService, appSettings, authorizationService)
        {

        }
    }
}
