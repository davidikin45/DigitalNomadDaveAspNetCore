﻿using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Interfaces.Services;
using DND.Domain.CMS.ContentHtmls.Dtos;
using DND.Interfaces.CMS.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.CMS.MVCImplementation.ContentHtml.Api
{
    [ApiVersion("1.0")]
    [Route("api/cms/content-htmls")]
    public class ContentHtmlsController : BaseEntityWebApiControllerAuthorize<ContentHtmlDto, ContentHtmlDto, ContentHtmlDto, ContentHtmlDeleteDto, IContentHtmlApplicationService>
    {
        public ContentHtmlsController(IContentHtmlApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, IConfiguration configuration)
            : base(service, mapper, emailService, urlHelper, typeHelperService, configuration)
        {

        }
    }
}