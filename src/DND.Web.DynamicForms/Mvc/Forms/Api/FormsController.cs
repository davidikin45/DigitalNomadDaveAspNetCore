using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Common.Interfaces.Services;
using DND.Domain.DynamicForms.Forms.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.DynamicForms.Mvc.Forms.Api
{
    [ApiVersion("1.0")]
    [Route("api/forms/forms")]
    public class FormsController : ApiControllerEntityAuthorizeBase<FormDto, FormDto, FormDto, FormDeleteDto, IFormApplicationService>
    {
        public FormsController(IFormApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, AppSettings appSettings, IAuthorizationService authorizationService)
            : base("forms.forms.", service, mapper, emailService, urlHelper, typeHelperService, appSettings, authorizationService)
        {

        }
    }
}
