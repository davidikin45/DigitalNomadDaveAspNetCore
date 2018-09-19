﻿using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.DynamicForms.Sections;
using DND.Domain.DynamicForms.Sections.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using DND.Interfaces.DynamicForms.DomainServices;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.DynamicForms.Sections.Services
{
    public class SectionApplicationService : ApplicationServiceEntityBase<Section, SectionDto, SectionDto, SectionDto, SectionDeleteDto, ISectionDomainService>, ISectionApplicationService
    {
        public SectionApplicationService(ISectionDomainService domainService, IMapper mapper, IHubContext<ApiNotificationHub<SectionDto>> hubContext)
        : base(domainService, mapper, hubContext)
        {

        }
    }
}
