using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Domain.DynamicForms.FormSectionSubmissions;
using DND.Domain.DynamicForms.FormSectionSubmissions.Dtos;
using DND.Interfaces.DynamicForms.ApplicationServices;
using DND.Interfaces.DynamicForms.DomainServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.DynamicForms.FormSectionSubmissions.Services
{
    public class FormSectionSubmissionApplicationService : ApplicationServiceEntityBase<FormSectionSubmission, FormSectionSubmissionDto, FormSectionSubmissionDto, FormSectionSubmissionDto, FormSectionSubmissionDeleteDto, IFormSectionSubmissionDomainService>, IFormSectionSubmissionApplicationService
    {
        public FormSectionSubmissionApplicationService(IFormSectionSubmissionDomainService domainService, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IHubContext<ApiNotificationHub<FormSectionSubmissionDto>> hubContext)
        : base("forms.form-sections.", domainService, mapper, authorizationService, userService, hubContext)
        {

        }
    }
}
