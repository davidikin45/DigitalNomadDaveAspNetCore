
using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.SignalRHubs;
using DND.Domain.DynamicForms.LookupTables;
using DND.Domain.DynamicForms.LookupTables.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using DND.Interfaces.DynamicForms.DomainServices;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.DynamicForms.LookupTables.Services
{
    public class LookupTableApplicationService : BaseEntityApplicationService<LookupTable, LookupTableDto, LookupTableDto, LookupTableDto, LookupTableDeleteDto, ILookupTableDomainService>, ILookupTableApplicationService
    {
        public LookupTableApplicationService(ILookupTableDomainService domainService, IMapper mapper, IHubContext<ApiNotificationHub<LookupTableDto>> hubContext)
        : base(domainService, mapper, hubContext)
        {

        }
    }
}
