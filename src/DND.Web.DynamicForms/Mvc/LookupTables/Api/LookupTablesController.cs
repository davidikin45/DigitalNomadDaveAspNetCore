using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Common.Interfaces.Services;
using DND.Domain.DynamicForms.LookupTables.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.DynamicForms.Mvc.LookupTables.Api
{
    [ApiVersion("1.0")]
    [Route("api/forms/lookup-tables")]
    public class LookupTablesController : ApiControllerEntityAuthorizeBase<LookupTableDto, LookupTableDto, LookupTableDto, LookupTableDeleteDto, ILookupTableApplicationService>
    {
        public LookupTablesController(ILookupTableApplicationService service, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, ITypeHelperService typeHelperService, AppSettings appSettings, IAuthorizationService authorizationService)
            : base("forms.lookup-tables.", service, mapper, emailService, urlHelper, typeHelperService, appSettings, authorizationService)
        {

        }
    }
}
