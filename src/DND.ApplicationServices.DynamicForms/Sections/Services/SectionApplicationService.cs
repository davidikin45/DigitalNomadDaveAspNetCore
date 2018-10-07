using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Domain.DynamicForms.Sections;
using DND.Domain.DynamicForms.Sections.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using DND.Interfaces.DynamicForms.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.DynamicForms.Sections.Services
{
    public class SectionApplicationService : ApplicationServiceEntityBase<Section, SectionDto, SectionDto, SectionDto, SectionDeleteDto, ISectionDomainService>, ISectionApplicationService
    {
        public SectionApplicationService(ISectionDomainService domainService, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IHubContext<ApiNotificationHub<SectionDto>> hubContext)
        : base("forms.sections.", domainService, mapper, authorizationService, userService, hubContext)
        {

        }
    }
}
