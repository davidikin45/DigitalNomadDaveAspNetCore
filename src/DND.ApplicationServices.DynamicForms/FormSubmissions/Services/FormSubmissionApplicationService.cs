﻿using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.SignalRHubs;
using DND.Domain.DynamicForms.FormSubmissions;
using DND.Domain.DynamicForms.FormSubmissions.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using DND.Interfaces.DynamicForms.DomainServices;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.DynamicForms.FormSubmissions.Services
{
    public class FormSubmissionApplicationService : BaseEntityApplicationService<FormSubmission, FormSubmissionDto, FormSubmissionDto, FormSubmissionDto, FormSubmissionDeleteDto, IFormSubmissionDomainService>, IFormSubmissionApplicationService
    {
        public FormSubmissionApplicationService(IFormSubmissionDomainService domainService, IMapper mapper, IHubContext<ApiNotificationHub<FormSubmissionDto>> hubContext)
        :base(domainService, mapper, hubContext)
        {

        }
    }
}