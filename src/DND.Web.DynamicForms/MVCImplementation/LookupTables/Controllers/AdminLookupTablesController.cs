using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Infrastructure.Email;
using DND.Domain.DynamicForms.LookupTables.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DND.Web.DynamicForms.MVCImplementation.LookupTables.Controllers
{
    [Route("admin/forms/lookup-tables")]
    public class AdminLookupTablesController : MvcControllerEntityAuthorizeBase<LookupTableDto, LookupTableDto, LookupTableDto, LookupTableDeleteDto, ILookupTableApplicationService>
    {
        public AdminLookupTablesController(ILookupTableApplicationService service, IMapper mapper, IEmailService emailService, IConfiguration configuration)
             : base(true, service, mapper, emailService, configuration)
        {
        }
    }
}
