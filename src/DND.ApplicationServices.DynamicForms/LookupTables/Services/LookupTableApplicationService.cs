
using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Domain.DynamicForms.LookupTables;
using DND.Domain.DynamicForms.LookupTables.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using DND.Interfaces.DynamicForms.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.DynamicForms.LookupTables.Services
{
    public class LookupTableApplicationService : ApplicationServiceEntityBase<LookupTable, LookupTableDto, LookupTableDto, LookupTableDto, LookupTableDeleteDto, ILookupTableDomainService>, ILookupTableApplicationService
    {
        public LookupTableApplicationService(ILookupTableDomainService domainService, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IHubContext<ApiNotificationHub<LookupTableDto>> hubContext)
        : base("forms.lookup-tables.", domainService, mapper, authorizationService, userService, hubContext)
        {

        }
    }
}
